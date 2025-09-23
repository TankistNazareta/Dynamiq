import { useState } from 'react';
import getUserIdFromAccessToken from '../../utils/services/getUserIdFromAccessToken';
import { addQuantityToCartItem } from '../../services/client/cart';

type Props = {
    id: string;
    quantity: number;
    className: string;
};

const AddCartBtn = ({ id, quantity: count, className }: Props) => {
    const [checkIds, setCheckIds] = useState<ReturnType<typeof setTimeout>[]>([]);

    const onAddToCart = () => {
        const resOfToken = getUserIdFromAccessToken();
        if (resOfToken.userId === undefined) {
            console.error('plese, log out and try again');
            return;
        }
        const timerId = setTimeout(() => {
            setCheckIds((prev) => prev.filter((id) => id !== timerId));
        }, 1000);
        setCheckIds((prev) => [...prev, timerId]);
        addQuantityToCartItem(resOfToken.userId, id, count);
    };

    return (
        <button onClick={onAddToCart} className={'btn--add-cart ' + className}>
            Add to cart
            {checkIds.map((id) => (
                <span key={id.toString()} className="btn--add-cart__check">
                    ✔
                </span>
            ))}
        </button>
    );
};

export default AddCartBtn;
