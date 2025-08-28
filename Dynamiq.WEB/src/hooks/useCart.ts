import { useEffect, useState } from 'react';
import { addItemToCart, CartRes, getCart, removeItemFromCart } from '../services/client/cart';
import getUserIdFromAccessToken from '../utils/services/getUserIdFromAccessToken';
import { ApiResult, ErrorMsgType } from '../utils/types/api';
import useHttpHook, { stateType } from './useHttp';
import { getByIdProduct, ProductResBody } from '../services/client/product';
import { useNavigate } from 'react-router-dom';
import { createCheckout, CreateCheckoutType } from '../services/client/payment';
import intervalEnum from '../utils/enums/intervalEnum';

export type CartItemData = {
    productId: string;
    quantity: number;
    name: string;
    price: number;
    img: string;
};

const useCart = (setLoaded: () => void, isLoaded: boolean) => {
    const [cartData, setCartData] = useState<CartItemData[]>([]);
    const { state, setState, makeRequest } = useHttpHook();
    const [error, setError] = useState('');

    useEffect(() => {
        const resOfToken = getUserIdFromAccessToken();

        if (resOfToken.error !== undefined) {
            setState('error');
            setError(resOfToken.error);
            return;
        }

        if (!isLoaded) {
            syncCart(resOfToken.userId);
            setLoaded();
        }

        const interval = setInterval(() => syncCart(resOfToken.userId), 30000);

        return () => clearInterval(interval);
    }, []);

    const syncCart = (userId: string) => {
        makeRequest<CartRes>(() => getCart(userId))
            .then(async (cartRes: CartRes) => {
                const arrayOfProducts = await Promise.all(
                    cartRes.items.map(async (item) => {
                        try {
                            const res = await makeRequest<ProductResBody>(() => getByIdProduct(item.productId));
                            return {
                                productId: item.productId,
                                quantity: item.quantity,
                                name: res.name,
                                price: res.price,
                                img: res.imgUrls[0].imgUrl,
                            } as CartItemData;
                        } catch (error) {
                            const err = error as ErrorMsgType;
                            if (err.StatusCode === 404) {
                                setState('loading');
                                await removeItemFromCart(userId, item.productId, item.quantity);
                            } else {
                                setError(err.Message);
                            }
                            return null;
                        }
                    })
                );

                return arrayOfProducts.filter((p): p is CartItemData => p !== null);
            })
            .then((products: CartItemData[]) => {
                setCartData(products);
                setState('success');
            })
            .catch((error) => {
                if (error.status === 404) {
                    setState('success');
                    return;
                }
                setError(error.Message);
            });
    };

    const onPurchase = (setUnloaded: Function, coupons?: string[]) => {
        const resOfToken = getUserIdFromAccessToken();

        if (resOfToken.error !== undefined) {
            setError(resOfToken.error);
            return;
        }

        setState('loading');
        setUnloaded();

        console.log(coupons);

        makeRequest<CreateCheckoutType>(() =>
            createCheckout({
                intervalEnum: intervalEnum.OneTime,
                userId: resOfToken.userId,
                couponCodes: coupons,
            })
        ).catch((err: ErrorMsgType) => {
            setError(err.Message);
        });
    };

    const onRemoveItem = async (productId: string, removeQuantity: number) => {
        const resOfToken = getUserIdFromAccessToken();

        if (resOfToken.error !== undefined) {
            setState('error');
            setError(resOfToken.error);
            return;
        }

        const item = cartData.find((prod) => prod.productId === productId)!;

        if (item.quantity - removeQuantity === 0) {
            setCartData(cartData.filter((item) => item.productId !== productId));
        } else {
            setCartData([
                ...cartData.filter((item) => item.productId !== productId),
                { ...item, quantity: item.quantity - removeQuantity },
            ]);
        }

        try {
            await makeRequest<CartRes>(() => removeItemFromCart(resOfToken.userId, productId, removeQuantity));
        } catch (ex) {
            if (item.quantity - removeQuantity === 0) {
                setCartData([...cartData, item]);
            } else {
                setCartData([
                    ...cartData.filter((item) => item.productId !== productId),
                    { ...item, quantity: item.quantity },
                ]);
            }
        }
    };

    const onAddItem = async (productId: string, addQuantity: number) => {
        const resOfToken = getUserIdFromAccessToken();

        if (resOfToken.error !== undefined) {
            setState('error');
            setError(resOfToken.error);
            return;
        }

        const item = cartData.find((prod) => prod.productId === productId)!;

        try {
            await makeRequest<CartRes>(() => addItemToCart(resOfToken.userId, productId, addQuantity));
        } catch (ex) {
            setCartData([
                ...cartData.filter((item) => item.productId !== productId),
                { ...item, quantity: item.quantity },
            ]);
        }
    };

    const onClearItem = (productId: string) => {
        onRemoveItem(productId, cartData.find((prod) => prod.productId === productId)!.quantity);
    };

    const onChangeQuantityInput = (productId: string, quantity: number) => {
        const notChangedItemQuantity = cartData.find((prod) => prod.productId === productId)!.quantity;

        if (quantity > notChangedItemQuantity) onAddItem(productId, quantity - notChangedItemQuantity);
        else if (quantity < notChangedItemQuantity) onRemoveItem(productId, notChangedItemQuantity - quantity);
    };

    return {
        onChangeQuantityInput,
        onClearItem,
        onAddItem,
        onRemoveItem,
        cartData,
        state,
        error,
        onPurchase,
    };
};

export default useCart;
