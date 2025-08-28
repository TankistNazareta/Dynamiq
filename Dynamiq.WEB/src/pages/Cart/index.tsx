import CartItem from './CartItem';

import img from '../../assets/images/testImgForCard.png';
import { useEffect, useState } from 'react';
import CouponItem from './CouponItem';
import Feature from '../../components/Feature';
import SubheaderNav from '../../components/SubheaderNav';
import useCart from '../../hooks/useCart';
import Loading from '../../components/Loading';
import { CouponRes, getCoupon } from '../../services/client/coupon';
import useHttpHook from '../../hooks/useHttp';
import PopupCoupon from './PopupCoupon';
import DiscountTypeEnum from '../../utils/enums/discountTypeEnum';

const Cart = () => {
    const [isLoaded, setIsLoaded] = useState(false);
    const [couponList, setCouponList] = useState<CouponRes[]>([]);
    const [needToShowPopupCoupon, setNeedToShowPopupCoupon] = useState(false);

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

    const countTotalDiscount = () => {
        let totalDiscount: number = 0;

        const percentageCoupon = couponList.filter((coupon) => coupon.discountType === DiscountTypeEnum.Percentage);
        const fixedCoupon = couponList.filter((coupon) => coupon.discountType === DiscountTypeEnum.FixedAmount);

        percentageCoupon.forEach((coupon) => {
            cartData.forEach(
                (cartItem) => (totalDiscount += cartItem.price * (coupon.discountValue / 100) * cartItem.quantity)
            );
        });

        fixedCoupon.forEach((coupon) => (totalDiscount += coupon.discountValue));

        return totalDiscount;
    };

    const totalDiscount = countTotalDiscount();
    const subTotal = cartData.reduce((acc, item) => acc + item.price * item.quantity, 0);

    return (
        <>
            <SubheaderNav nameRoute="Cart" />
            <section className="cart container d-flex flex-wrap justify-content-between">
                <div className="cart__list">
                    <div className="cart__hat d-flex align-items-center justify-content-between flex-wrap">
                        <p className="cart__hat_descr">Your cart:</p>
                        <p>Name:</p>
                        <p>Price:</p>
                        <p>Quantity:</p>
                    </div>
                    <div className="cart__wrapper d-flex flex-column">
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
                                    priceTotal={data.price}
                                    onChangeQuantity={(num: number) => onChangeQuantityInput(data.productId, num)}
                                    onDelete={() => onClearItem(data.productId)}
                                />
                            ))
                        ) : (
                            <h3 className="cart__wrapper_title">Your cart is empty</h3>
                        )}
                    </div>
                    {cartData.length !== 0 && (
                        <>
                            <div className="cart__hat cart__hat-coupon d-flex align-items-center justify-content-between flex-wrap">
                                <p className="cart__hat_descr">Your coupons:</p>
                                <p>Code:</p>
                                <p>End date:</p>
                                <p>Amount:</p>
                            </div>
                            <div className="cart__wrapper d-flex flex-column align-items-center">
                                {couponList.length ? (
                                    couponList.map((coupon) => (
                                        <CouponItem
                                            code={coupon.code}
                                            key={coupon.id}
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
                                    <h3 className="cart__wrapper_title">You don't have active coupons</h3>
                                )}
                                <button className="cart__add-new-coupon" onClick={() => setNeedToShowPopupCoupon(true)}>
                                    Add new coupon
                                </button>
                            </div>
                        </>
                    )}
                </div>
                <div className="cart__checkout">
                    <h3 className="cart__checkout_title">Cart Totals</h3>
                    <div className="cart__checkout__text d-flex justify-content-between align-items-center">
                        <h4 className="cart__checkout__text_title">Subtotal</h4>
                        <h5 className="cart__checkout__text-descr">${subTotal}</h5>
                    </div>
                    <div className="cart__checkout__text d-flex justify-content-between align-items-center">
                        <h4 className="cart__checkout__text_title">Discount</h4>
                        <h5
                            className={`cart__checkout__text-descr ${
                                totalDiscount > 0 ? 'cart__checkout__text-discount' : ''
                            }`}>
                            {totalDiscount}$
                        </h5>
                    </div>
                    <div className="cart__checkout__text d-flex justify-content-between align-items-center">
                        <h4 className="cart__checkout__text_title">Total:</h4>
                        <h5 className="cart__checkout__text-price">${subTotal - totalDiscount}</h5>
                    </div>
                    <button
                        className="cart__checkout__btn"
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
