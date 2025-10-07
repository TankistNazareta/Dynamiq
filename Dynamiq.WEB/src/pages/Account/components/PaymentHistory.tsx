import '../scss/paymentHistory.scss';

import { useState } from 'react';
import useHttpHook from '../../../hooks/useHttp';
import { getByIdProduct, ProductResBody } from '../../../services/client/product';
import { ErrorMsgType } from '../../../utils/types/api';
import Loading from '../../../components/Loading';
import { ProductPaymentHistory } from '../../../services/client/paymentHistory';
import parseDateTime from '../../../utils/services/parseDateTime';
import { Link } from 'react-router-dom';

interface PaymentHistoryProps {
    amount: number;
    createdAt: Date;
    productPaymentHistory: ProductPaymentHistory[];
}

type ProductPaymentHistoryWithAllData = {
    name: string;
    img: string;
    quantity: number;
    productId: string;
};

const PaymentHistory: React.FC<PaymentHistoryProps> = ({ amount, createdAt, productPaymentHistory }) => {
    const [needToShowDetails, setNeedToShowDetails] = useState(false);
    const [productPaymentHistoryData, setProductPaymentHistoryData] = useState<ProductPaymentHistoryWithAllData[]>([]);
    const [error, setError] = useState('');

    const { state, setState, makeRequest } = useHttpHook();

    const onLoad = async () => {
        if (productPaymentHistoryData.length) return;

        setState('loading');

        const array: ProductPaymentHistoryWithAllData[] = [];

        try {
            for (const prodPayment of productPaymentHistory) {
                const product = await makeRequest<ProductResBody>(() => getByIdProduct(prodPayment.productId));
                array.push({
                    name: product.name,
                    img: product.imgUrls[0].imgUrl,
                    quantity: prodPayment.quantity,
                    productId: prodPayment.productId,
                });
            }

            setProductPaymentHistoryData(array);
            setState('success');
        } catch (error) {
            setError((error as ErrorMsgType).Message);
            setState('error');
        }
    };

    return (
        <div className={`payment-history-item ${needToShowDetails ? 'payment-history-item-active' : ''}`}>
            <div
                className="payment-history-item__wrapper"
                onClick={() => {
                    setNeedToShowDetails(!needToShowDetails);
                    onLoad();
                }}>
                <p className="payment-history-item__descr">${amount}</p>
                <p className="payment-history-item__descr">{parseDateTime(createdAt)}</p>
                <button
                    className={`payment-history-item__descr payment-history-item__btn btn--right-arrow ${
                        needToShowDetails ? 'btn--right-arrow--active' : ''
                    }`}>
                    More
                    <span></span>
                    <span></span>
                </button>
            </div>
            {needToShowDetails && (
                <>
                    <hr className="hr-separator payment-history-item__hr-separator" />
                    <div className="payment-history-item__product-wrapper">
                        {state === 'error' ? (
                            <h3 className="title-error text-danger">{error}</h3>
                        ) : state === 'success' ? (
                            productPaymentHistoryData.map((obj, idx) => (
                                <Link key={obj.productId ?? idx} to={`/product/${obj.productId}`}>
                                    <div className="cart-item  payment-history-item__product">
                                        <div className="cart-item__img payment-history-item__product-img">
                                            <img src={obj.img} alt="" />
                                        </div>
                                        <h5 className="cart-item__title cart-item__title-name">{obj.name}</h5>
                                        <input
                                            type="text"
                                            value={obj.quantity || 0}
                                            onChange={() => {}}
                                            className="cart-item__quantity"
                                            disabled
                                        />
                                    </div>
                                </Link>
                            ))
                        ) : (
                            <Loading />
                        )}
                    </div>
                </>
            )}
        </div>
    );
};

export default PaymentHistory;
