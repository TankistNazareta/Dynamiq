import { useState } from 'react';

import imgTest from '../../../assets/images/testImgForCard.png';

interface PaymentHistoryProps {
    amount: number;
    createdAt: Date;
}

const PaymentHistory: React.FC<PaymentHistoryProps> = ({ amount, createdAt }) => {
    const [needToShowDetails, setNeedToShowDetails] = useState(false);
    const array = [
        { name: 'name', img: imgTest, price: 98, quantity: 2 },
        { name: 'name', img: imgTest, price: 98, quantity: 2 },
        { name: 'name', img: imgTest, price: 98, quantity: 2 },
    ];

    return (
        <div className={`user__payment-history_item ${needToShowDetails ? 'user__payment-history_item-active' : ''}`}>
            <div
                className="user__payment-history_item__wrapper d-flex align-items-center justify-content-around"
                onClick={() => setNeedToShowDetails(!needToShowDetails)}>
                <p className="user__payment-history_item_descr">${amount}</p>
                <p className="user__payment-history_item_descr">
                    {createdAt.toLocaleString('en-GB', {
                        month: 'short',
                        day: '2-digit',
                        hour: '2-digit',
                        minute: '2-digit',
                        hour12: false,
                    })}
                </p>
                <button
                    className={`user__payment-history_item_descr user__payment-history_item_btn ${
                        needToShowDetails ? 'user__payment-history_item_btn-active' : ''
                    }`}>
                    More
                    <span></span>
                    <span></span>
                </button>
            </div>
            {needToShowDetails ? (
                <>
                    <hr className="hr-separator user__payment-history_item_hr-separator" />
                    <div className="user__payment-history_item_product__wrapper d-flex flex-column">
                        {array.map((obj) => (
                            <div className="cart__item d-flex align-items-center user__payment-history_item_product">
                                <div className="cart__item_img user__payment-history_item_product_img">
                                    <img src={obj.img} alt="" />
                                </div>
                                <h5 className="cart__item_title cart__item_title-name">{obj.name}</h5>
                                <input
                                    type="text"
                                    value={obj.quantity || 0}
                                    onChange={(e) => {}}
                                    className="cart__item_quantity"
                                    disabled
                                />
                            </div>
                        ))}
                    </div>
                </>
            ) : (
                ''
            )}
        </div>
    );
};

export default PaymentHistory;
