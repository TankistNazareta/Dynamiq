import './scss/account.scss';

import { useEffect, useState } from 'react';
import useHttpHook from '../../hooks/useHttp';
import { getUserById, UserRes } from '../../services/client/user';
import roleEnum from '../../utils/enums/roleEnum';
import AdminPanel from './components/AdminPanel';
import PaymentHistory from './components/PaymentHistory';
import { ErrorMsgType } from '../../utils/types/api';
import Loading from '../../components/Loading';
import { getPaymentHistoryByUserId, PaymentHistoryRes } from '../../services/client/paymentHistory';
import getUserIdFromAccessToken from '../../utils/services/getUserIdFromAccessToken';
import SubscriptionModal from './components/SubscriptionModal';

const Account = () => {
    const [error, setError] = useState('');
    const [user, setUser] = useState<UserRes>();

    const { state, setState, makeRequest } = useHttpHook();

    useEffect(() => {
        if (user !== undefined) return;

        const resOfToken = getUserIdFromAccessToken();

        if (resOfToken.error !== undefined) {
            setError(resOfToken.error);
            return;
        }

        makeRequest<UserRes>(() => getUserById(resOfToken.userId))
            .then(async (userRes: UserRes) => {
                const paymentHistoryRes = await makeRequest<PaymentHistoryRes[]>(() =>
                    getPaymentHistoryByUserId(resOfToken.userId)
                );
                setUser({ ...userRes, paymnetHistories: paymentHistoryRes });
            })
            .then(() => setState('success'))
            .catch((error: ErrorMsgType) => setError(error.Message));
    }, []);

    if (state === 'success') return <View user={user!} />;
    else if (state === 'error') return <h3 className="title-error text-danger">{error}</h3>;

    return <Loading />;
};

const View: React.FC<{ user: UserRes }> = ({ user }) => {
    const [needShowSubscriptionModal, setNeedShowSubscriptionModal] = useState(false);

    return (
        <>
            <section className="user__about">
                <h2 className="user__about-welcome">Welcome back!</h2>
                <h5 className="user__about-descr">
                    Your role - {user.role === roleEnum.Admin ? 'Admin' : 'default user'}. <br />
                    Your email - {user.email}. <br />
                    You
                    {user.hasActiveSubscription ? ' have' : " don't have"} subscribtion
                    {!user.hasActiveSubscription ? (
                        <button className="user__about-buy" onClick={() => setNeedShowSubscriptionModal(true)}>
                            <strong>Buy subscribtion?</strong>
                        </button>
                    ) : (
                        ''
                    )}
                    <br /> We was waiting for you
                </h5>
            </section>
            <section className="payment-history container">
                <div className="payment-history__header ">Your payment history:</div>
                <div className="payment-history__container">
                    {user.paymnetHistories.length ? (
                        user.paymnetHistories
                            .filter((item) => item.products.length)
                            .map((item, idx) => (
                                <PaymentHistory
                                    key={item.id ?? idx}
                                    amount={item.amount}
                                    createdAt={item.createdAt}
                                    productPaymentHistory={item.products}
                                />
                            ))
                    ) : (
                        <h3 className="payment-history__descr">You don't have yet payment histories</h3>
                    )}
                </div>
            </section>
            {user.role === roleEnum.Admin ? <AdminPanel /> : ''}
            <SubscriptionModal isOpen={needShowSubscriptionModal} onClose={() => setNeedShowSubscriptionModal(false)} />
        </>
    );
};

export default Account;
