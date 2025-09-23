import { Link } from 'react-router-dom';
import CartItemProps from '../../../utils/types/components/CartItemProps';
import { useState } from 'react';

const CartItem: React.FC<CartItemProps> = ({
    imgUrl,
    name,
    priceTotal,
    quantity,
    onChangeQuantity,
    onSetQuantity,
    onDelete,
    productId,
}) => {
    const [customQuantity, setCustomQuantity] = useState(quantity);

    return (
        <div className="cart-item">
            <Link to={`/product/${productId}`}>
                <div className="cart-item__img">
                    <img src={imgUrl} alt="" />
                </div>
            </Link>
            <h5 className="cart-item__title cart-item__title-name">{name}</h5>
            <h5 className="cart-item__title cart-item__title-price">${priceTotal}</h5>
            <input
                type="text"
                value={customQuantity || 0}
                onChange={(e) => {
                    const onlyNums = e.target.value.replace(/\D/g, '');
                    setCustomQuantity(Number.parseInt(onlyNums));
                    onChangeQuantity(Number.parseInt(onlyNums) || 0);
                }}
                onBlur={(e) => {
                    const onlyNums = e.target.value.replace(/\D/g, '');
                    onSetQuantity(Number.parseInt(onlyNums) || 0);
                }}
                className="cart-item__quantity"
            />
            <button className="cart-item__remove" onClick={() => onDelete()}>
                <svg width="21" height="22" viewBox="0 0 21 22" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path
                        d="M20.125 3.9375H16.625V1.75C16.625 0.784766 15.8402 0 14.875 0H6.125C5.15977 0 4.375 0.784766 4.375 1.75V3.9375H0.875C0.391016 3.9375 0 4.32852 0 4.8125V5.6875C0 5.80781 0.0984375 5.90625 0.21875 5.90625H1.87031L2.5457 20.207C2.58945 21.1395 3.36055 21.875 4.29297 21.875H16.707C17.6422 21.875 18.4105 21.1422 18.4543 20.207L19.1297 5.90625H20.7812C20.9016 5.90625 21 5.80781 21 5.6875V4.8125C21 4.32852 20.609 3.9375 20.125 3.9375ZM14.6562 3.9375H6.34375V1.96875H14.6562V3.9375Z"
                        fill="#B88E2F"
                    />
                </svg>
            </button>
        </div>
    );
};

export default CartItem;
