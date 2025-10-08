import './scss/cart.scss';

import CartItem from './components/CartItem';
import { useEffect, useState } from 'react';
import CouponItem from './components/CouponItem';
import Feature from '../../components/Feature';
import SubheaderNav from '../../components/SubheaderNav';
import useCart, { CartItemData } from '../../hooks/useCart';
import Loading from '../../components/Loading';
import { CouponRes, getCoupon } from '../../services/client/coupon';
import PopupCoupon from './components/PopupCoupon';
import DiscountTypeEnum from '../../utils/enums/discountTypeEnum';

const Cart = () => {
    const [isLoaded, setIsLoaded] = useState(false);
    const [couponList, setCouponList] = useState<CouponRes[]>([]);
    const [needToShowPopupCoupon, setNeedToShowPopupCoupon] = useState(false);
    const [subTotal, setSubTotal] = useState(0);
    const [totalDiscount, setTotalDiscount] = useState(0);

    const {
        onChangeQuantityInput,
        onClearItem,
        cartData,
        state: stateCart,
        error: errorCart,
        onPurchase,
    } = useCart(() => setIsLoaded(true), isLoaded);

    const onApplyCoupon = (coupon: CouponRes) => {
        if (couponList.find((item) => item.id === coupon.id) !== undefined) return;

        setCouponList((prev) => [...prev, coupon]);
    };

    useEffect(() => {
        setTotalDiscount(countTotalDiscount(cartData));
    }, [couponList]);

    useEffect(() => {
        if (subTotal !== 0) return;

        setSubTotal(cartData.reduce((acc, item) => acc + item.price * item.quantity, 0));
        setTotalDiscount(countTotalDiscount());
    }, [cartData]);

    const onChangeQuantity = (productId: string, quantity: number) => {
        setSubTotal(
            cartData.reduce((acc, item) => {
                if (item.productId === productId) return acc + item.price * quantity;

                return acc + item.price * item.quantity;
            }, 0)
        );
        setTotalDiscount(countTotalDiscount(cartData));
    };

    const countTotalDiscount = (cartDataProp: CartItemData[] = cartData) => {
        let totalDiscount: number = 0;

        const percentageCoupon = couponList.filter((coupon) => coupon.discountType === DiscountTypeEnum.Percentage);
        const fixedCoupon = couponList.filter((coupon) => coupon.discountType === DiscountTypeEnum.FixedAmount);

        percentageCoupon.forEach((coupon) => {
            cartDataProp.forEach(
                (cartItem) => (totalDiscount += cartItem.price * (coupon.discountValue / 100) * cartItem.quantity)
            );
        });

        fixedCoupon.forEach((coupon) => (totalDiscount += coupon.discountValue));

        if (totalDiscount > subTotal) return subTotal;

        return Math.trunc(totalDiscount * 100) / 100;
    };

    return (
        <>
            <SubheaderNav nameRoute="Cart" />
            <section className="cart container">
                <div className="cart__list">
                    <div className="cart__hat">
                        <p className="cart__hat-descr">Your cart:</p>
                        <p>Name:</p>
                        <p>Price:</p>
                        <p>Quantity:</p>
                    </div>
                    <div className="cart__wrapper">
                        {stateCart === 'error' ? (
                            <h3 className="title-error text-danger">{errorCart}</h3>
                        ) : !isLoaded ? (
                            <Loading />
                        ) : cartData.length ? (
                            cartData.map((data) => (
                                <CartItem
                                    key={data.productId}
                                    productId={data.productId}
                                    imgUrl={data.img}
                                    name={data.name}
                                    quantity={data.quantity}
                                    priceTotal={data.price * data.quantity}
                                    onChangeQuantity={(num: number) => onChangeQuantity(data.productId, num)}
                                    onDelete={() => onClearItem(data.productId)}
                                    onSetQuantity={(quantity: number) =>
                                        onChangeQuantityInput(data.productId, quantity)
                                    }
                                />
                            ))
                        ) : (
                            <h3 className="cart__wrapper-title">Your cart is empty</h3>
                        )}
                    </div>
                    {cartData.length !== 0 && (
                        <>
                            <div className="cart__hat cart__hat-coupon">
                                <p className="cart__hat-descr">Your coupons:</p>
                                <p>Code:</p>
                                <p>End date:</p>
                                <p>Amount:</p>
                            </div>
                            <div className="cart__wrapper">
                                {couponList.length ? (
                                    couponList.map((coupon, i) => (
                                        <CouponItem
                                            code={coupon.code}
                                            key={coupon.id}
                                            place={i + 1}
                                            amount={
                                                coupon.discountType === DiscountTypeEnum.FixedAmount
                                                    ? `${coupon.discountValue}$`
                                                    : `${coupon.discountValue}%`
                                            }
                                            endTime={coupon.endTime}
                                            onDelete={() =>
                                                setCouponList((prev) =>
                                                    prev.filter((item) => item.code !== coupon.code)
                                                )
                                            }
                                        />
                                    ))
                                ) : (
                                    <h3 className="cart__wrapper-title">You don't have active coupons</h3>
                                )}
                                <button className="cart__add-new-coupon" onClick={() => setNeedToShowPopupCoupon(true)}>
                                    Add new coupon
                                </button>
                            </div>
                        </>
                    )}
                </div>
                <div className="cart__checkout">
                    <h3 className="cart__checkout-title">Cart Totals</h3>
                    <div className="cart__checkout-text">
                        <h4 className="cart__checkout-text-title">Subtotal:</h4>
                        <h5 className="cart__checkout-text-descr">${subTotal}</h5>
                    </div>
                    <div className="cart__checkout-text">
                        <h4 className="cart__checkout-text-title">Discount:</h4>
                        <h5
                            className={`cart__checkout-text-descr ${
                                totalDiscount > 0 && 'cart__checkout-text-discount'
                            }`}>
                            {totalDiscount}$
                        </h5>
                    </div>
                    <div className="cart__checkout-text">
                        <h4 className="cart__checkout-text-title">Total:</h4>
                        <h5 className="cart__checkout-text-price">
                            ${Math.trunc((subTotal - totalDiscount) * 100) / 100}
                        </h5>
                    </div>
                    <button
                        className="cart__checkout-btn"
                        onClick={() =>
                            onPurchase(
                                () => setIsLoaded(false),
                                couponList.map((coupon) => coupon.code)
                            )
                        }>
                        Check Out
                    </button>
                </div>
            </section>
            <Feature />
            <PopupCoupon
                show={needToShowPopupCoupon}
                onClose={() => setNeedToShowPopupCoupon(false)}
                onApply={onApplyCoupon}
            />
        </>
    );
};

export default Cart;
