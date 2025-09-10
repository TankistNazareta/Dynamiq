import { useState } from 'react';
import { Link } from 'react-router-dom';
import getUserIdFromAccessToken from '../../utils/services/getUserIdFromAccessToken';
import { addQuantityToCartItem } from '../../services/client/cart';

interface CardProps {
    additionalClasses?: string;
    name: string;
    price: number;
    descr: string;
    imgUrl: string;
    id: string;
}

const Card: React.FC<CardProps> = ({ additionalClasses, name, price, descr, imgUrl, id }) => {
    const [needToShowPopUp, setNeedToShowPopUp] = useState<boolean>(false);

    const onAddToCart = () => {
        const resOfToken = getUserIdFromAccessToken();

        if (resOfToken.userId === undefined) {
            console.error('plese, log out and try again');
            return;
        }

        addQuantityToCartItem(resOfToken.userId, id, 1);
    };
    return (
        <div
            className={`card ${additionalClasses || ''}`}
            onMouseEnter={() => setNeedToShowPopUp(true)}
            onMouseLeave={() => setNeedToShowPopUp(false)}
            onTouchStart={() => setNeedToShowPopUp(true)}
            onTouchCancel={() => setNeedToShowPopUp(false)}>
            <img className="card-img" src={imgUrl} alt="" />
            <div className="card__info">
                <h3 className="card__info-title">{name}</h3>
                <p className="card__info-descr">{descr}</p>
                <h4 className="card__info-price">${price}</h4>
            </div>
            <div
                className={`card__popup flex-column justify-content-center align-items-center ${
                    needToShowPopUp ? ' card__popup-active' : ''
                }`}>
                <button className="card__popup-btn" onClick={() => onAddToCart()}>
                    Add to cart
                </button>
                <div className="card__popup-links d-flex justify-content-between">
                    <a href="" className="card__popup-link d-flex align-items-center">
                        <svg width="12" height="14" viewBox="0 0 12 14" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path
                                d="M10 9.66671C9.47467 9.66671 9 9.87337 8.644 10.2047L3.94 7.46671C3.97333 7.31337 4 7.16004 4 7.00004C4 6.84004 3.97333 6.68671 3.94 6.53337L8.64 3.79337C9 4.12671 9.47333 4.33337 10 4.33337C11.1067 4.33337 12 3.44004 12 2.33337C12 1.22671 11.1067 0.333374 10 0.333374C8.89333 0.333374 8 1.22671 8 2.33337C8 2.49337 8.02667 2.64671 8.06 2.80004L3.36 5.54004C3 5.20671 2.52667 5.00004 2 5.00004C0.893333 5.00004 0 5.89337 0 7.00004C0 8.10671 0.893333 9.00004 2 9.00004C2.52667 9.00004 3 8.79337 3.36 8.46004L8.05867 11.2054C8.02112 11.3563 8.00143 11.5112 8 11.6667C8 12.0623 8.1173 12.4489 8.33706 12.7778C8.55682 13.1067 8.86918 13.3631 9.23463 13.5145C9.60009 13.6658 10.0022 13.7054 10.3902 13.6283C10.7781 13.5511 11.1345 13.3606 11.4142 13.0809C11.6939 12.8012 11.8844 12.4448 11.9616 12.0569C12.0387 11.6689 11.9991 11.2668 11.8478 10.9013C11.6964 10.5359 11.44 10.2235 11.1111 10.0038C10.7822 9.784 10.3956 9.66671 10 9.66671Z"
                                fill="white"
                            />
                        </svg>
                        <p>Share</p>
                    </a>
                    <Link to={'/product/' + id} className="card__popup-link d-flex align-items-center">
                        <svg
                            width="20px"
                            height="20px"
                            viewBox="0 0 18 20"
                            fill="none"
                            xmlns="http://www.w3.org/2000/svg"
                            stroke="#ffffff">
                            <g id="SVGRepo_bgCarrier" stroke-width="0"></g>
                            <g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g>
                            <g id="SVGRepo_iconCarrier">
                                <path
                                    d="M12.01 19V9M12.01 5H12"
                                    stroke="#ffffff"
                                    stroke-width="2"
                                    stroke-linecap="round"
                                    stroke-linejoin="round"></path>
                            </g>
                        </svg>
                        <p>Detail</p>
                    </Link>
                </div>
            </div>
        </div>
    );
};

export default Card;
