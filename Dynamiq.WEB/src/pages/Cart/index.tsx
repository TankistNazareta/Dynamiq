import CartItem from './CartItem';

import img from '../../assets/images/testImgForCard.png';
import { useState } from 'react';
import CouponItem from './CouponItem';
import Feature from '../../components/Feature';
import SubheaderNav from '../../components/SubheaderNav';

interface CartProps {
    discount: number;
}

const Cart: React.FC<CartProps> = ({ discount }) => {
    const [quantity, setQuantity] = useState(1);

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
                        <CartItem
                            img={img}
                            name="Asgaard sofa"
                            price={200}
                            quantity={quantity}
                            setQuantity={(int: number) => setQuantity(int)}
                            onRemove={() => console.log('onRemove')}
                        />
                        <CartItem
                            img={img}
                            name="Asgaard sofa"
                            price={200}
                            quantity={quantity}
                            setQuantity={(int: number) => setQuantity(int)}
                            onRemove={() => console.log('onRemove')}
                        />
                        <CartItem
                            img={img}
                            name="Asgaard sofa"
                            price={200}
                            quantity={quantity}
                            setQuantity={(int: number) => setQuantity(int)}
                            onRemove={() => console.log('onRemove')}
                        />
                    </div>
                    <div className="cart__hat cart__hat-coupon d-flex align-items-center justify-content-between flex-wrap">
                        <p className="cart__hat_descr">Your coupons:</p>
                        <p>Code:</p>
                        <p>End date:</p>
                        <p>Amount:</p>
                    </div>
                    <div className="cart__wrapper d-flex flex-column align-items-center">
                        <CouponItem
                            code="Asgaard sofa"
                            amount={200}
                            endTime={new Date()}
                            onDelete={() => console.log('onRemove')}
                        />
                        <button className="cart__add-new-coupon">Add new coupon</button>
                    </div>
                </div>
                <div className="cart__checkout">
                    <h3 className="cart__checkout_title">Cart Totals</h3>
                    <div className="cart__checkout__text d-flex justify-content-between align-items-center">
                        <h4 className="cart__checkout__text_title">Subtotal</h4>
                        <h5 className="cart__checkout__text-descr">Rs. 250,000.00</h5>
                    </div>
                    <div className="cart__checkout__text d-flex justify-content-between align-items-center">
                        <h4 className="cart__checkout__text_title">Discount</h4>
                        <h5
                            className={`cart__checkout__text-descr ${
                                discount > 0 ? 'cart__checkout__text-discount' : ''
                            }`}>
                            {discount}
                        </h5>
                    </div>
                    <div className="cart__checkout__text d-flex justify-content-between align-items-center">
                        <h4 className="cart__checkout__text_title">Total</h4>
                        <h5 className="cart__checkout__text-price">Rs. 250,000.00</h5>
                    </div>
                    <button className="cart__checkout__btn">Check Out</button>
                </div>
            </section>
            <Feature />
        </>
    );
};

export default Cart;
