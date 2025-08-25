import { useState } from 'react';

import imgTest from '../../../assets/images/testImgForCard.png';
import useHttpHook, { stateType } from '../../../hooks/useHttp';
import { getByIdProduct, ProductRes, ProductResBody } from '../../../services/client/product';
import { ErrorMsgType } from '../../../utils/types/api';
import Loading from '../../../components/Loading';
import { ProductPaymentHistory } from '../../../services/client/paymentHistory';
import parseDateTime from '../../../utils/services/parseDateTime';

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
        <div className={`user__payment-history_item ${needToShowDetails ? 'user__payment-history_item-active' : ''}`}>
            <div
                className="user__payment-history_item__wrapper d-flex align-items-center justify-content-around"
                onClick={() => {
                    setNeedToShowDetails(!needToShowDetails);
                    onLoad();
                }}>
                <p className="user__payment-history_item_descr">${amount}</p>
                <p className="user__payment-history_item_descr">{parseDateTime(createdAt)}</p>
                <button
                    className={`user__payment-history_item_descr user__payment-history_item_btn ${
                        needToShowDetails ? 'user__payment-history_item_btn-active' : ''
                    }`}>
                    More
                    <span></span>
                    <span></span>
                </button>
            </div>
            {needToShowDetails && (
                <>
                    <hr className="hr-separator user__payment-history_item_hr-separator" />
                    <div className="user__payment-history_item_product__wrapper d-flex flex-column">
                        {state === 'error' ? (
                            <h3 className="title-error text-danger">{error}</h3>
                        ) : state === 'success' ? (
                            productPaymentHistoryData.map((obj) => (
                                <div
                                    key={obj.productId}
                                    className="cart__item d-flex align-items-center user__payment-history_item_product">
                                    <div className="cart__item_img user__payment-history_item_product_img">
                                        <img src={obj.img} alt="" />
                                    </div>
                                    <h5 className="cart__item_title cart__item_title-name">{obj.name}</h5>
                                    <input
                                        type="text"
                                        value={obj.quantity || 0}
                                        onChange={() => {}}
                                        className="cart__item_quantity"
                                        disabled
                                    />
                                </div>
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
