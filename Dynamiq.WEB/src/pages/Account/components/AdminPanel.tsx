import '../scss/adminPanel.scss';

import { useState } from 'react';
import PaymentHistory from './PaymentHistory';
import { getUserByEmail, UserRes } from '../../../services/client/user';
import useHttpHook from '../../../hooks/useHttp';
import { ErrorMsgType } from '../../../utils/types/api';
import Loading from '../../../components/Loading';
import parseDateTime from '../../../utils/services/parseDateTime';
import { getPaymentHistoryByUserEmail, PaymentHistoryRes } from '../../../services/client/paymentHistory';

const AdminPanel = () => {
    const [inputEmail, setInputEmail] = useState<string>('');
    const [user, setUser] = useState<UserRes>();
    const [error, setError] = useState('');

    const { state, setState, makeRequest } = useHttpHook();

    const onSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        if (!inputEmail) {
            setError('email cannot be null');
            return;
        }

        makeRequest<UserRes>(() => getUserByEmail(inputEmail))
            .then(async (userRes: UserRes) => {
                const paymentHistoryRes = await makeRequest<PaymentHistoryRes[]>(() =>
                    getPaymentHistoryByUserEmail(inputEmail)
                );
                setUser({ ...userRes, paymnetHistories: paymentHistoryRes });
            })
            .then(() => setState('success'))
            .catch((error: ErrorMsgType) => setError(error.Message));
    };

    return (
        <section className="admin-panel container">
            <h4 className="admin-panel__title">Your admin panel</h4>
            <p className="admin-panel__descr">Here you can write somebody's email and you'll get his info</p>
            <form
                action=""
                className="admin-panel__form d-flex justify-content-center fle-wrap"
                onSubmit={(e) => onSubmit(e)}>
                <input
                    value={inputEmail}
                    onChange={(e) => setInputEmail(e.target.value)}
                    type="text"
                    className="admin-panel__form_input"
                    placeholder="Email"
                />
                <button type="submit" className="admin-panel__form-btn">
                    Submit
                </button>
            </form>
            {state === 'loading' ? (
                <Loading />
            ) : state === 'error' ? (
                <h3 className="title-error text-danger">{error}</h3>
            ) : state === 'success' ? (
                user ? (
                    <View user={user} />
                ) : (
                    <h3 className="title-error text-danger">User not found</h3>
                )
            ) : null}
        </section>
    );
};

const View: React.FC<{ user: UserRes }> = ({ user }) => {
    return (
        <>
            <div className="admin-panel__result d-flex flex-wrap justify-content-around">
                <p className="payment-history-item__descr admin-panel__result-descr">{user.email}</p>
                <p className="payment-history-item__descr admin-panel__result-descr">
                    {'Created at: '}
                    {parseDateTime(user.emailVerification.createdAt)}
                </p>
                <p className="payment-history-item__descr admin-panel__result-descr">
                    email confirmed: {user.emailVerification.isConfirmed ? '✔' : '❌'}
                </p>
                <p className="payment-history-item__descr admin-panel__result-descr">
                    has subscription: {user.subscription.isConfirmed ? '✔' : '❌'}
                </p>
                <p className="payment-history-item__descr admin-panel__result-payment-history">Payment Histories:</p>
                <div className="payment-history__container d-flex flex-column admin-panel__result-payment-history-container container">
                    {user.paymnetHistories && user.paymnetHistories.length ? (
                        user.paymnetHistories.map((item, idx) => (
                            <PaymentHistory
                                key={item.id ?? idx}
                                amount={item.amount}
                                createdAt={item.createdAt}
                                productPaymentHistory={item.products}
                            />
                        ))
                    ) : (
                        <h3 className="payment-history__descr text-center">User doesn't have yet payment histories</h3>
                    )}
                </div>
            </div>
        </>
    );
};

export default AdminPanel;
