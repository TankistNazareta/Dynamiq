import { useEffect, useState } from 'react';
import useHttpHook from '../../hooks/useHttp';
import { getUserById, UserRes } from '../../services/client/user';
import roleEnum from '../../utils/enums/roleEnum';
import AdminPanel from './components/AdminPanel';
import PaymentHistory from './components/PaymentHistory';
import { jwtDecode } from 'jwt-decode';
import { ErrorMsgType } from '../../utils/types/api';
import Loading from '../../components/Loading';
import { getPaymentHistoryByUserId, PaymentHistoryRes } from '../../services/client/paymentHistory';

interface AccountProps {
    role: roleEnum;
    email: string;
    hasSubscription: boolean;
}

const Account: React.FC<AccountProps> = ({ role, email, hasSubscription }) => {
    const [error, setError] = useState('');
    const [user, setUser] = useState<UserRes>();

    const { state, setState, makeRequest } = useHttpHook();

    useEffect(() => {
        if (user !== undefined) return;

        const token = localStorage.getItem('token');

        if (!token) {
            setError('plese logIn before go to your account');
            return;
        }

        type Payload = { sub?: string; id?: string; userId?: string; [k: string]: any };

        const payload = jwtDecode<Payload>(token!);
        const userId = payload.sub ?? payload.id ?? payload.userId;

        if (!userId) {
            setError('plese re-logIn, because your token have problems');
            return;
        }

        makeRequest<UserRes>(() => getUserById(userId))
            .then(async (userRes: UserRes) => {
                const paymentHistoryRes = await makeRequest<PaymentHistoryRes[]>(() =>
                    getPaymentHistoryByUserId(userId)
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
    return (
        <>
            <section className="user__about">
                <h2 className="user__about_welcome">Welcome back!</h2>
                <h5 className="user__about_descr">
                    Your role - {user.role == roleEnum.Admin ? 'Admin' : 'default user'}. <br />
                    Your email - {user.email}. <br />
                    You
                    {user.subscription.isConfirmed ? ' have' : " don't have"} subscribtion <br /> We was waiting for you
                </h5>
            </section>
            <section className="user__payment-history container">
                <div className="user__payment-history__header d-flex align-items-center">Your payment history:</div>
                <div className="user__payment-history__container d-flex flex-column">
                    {user.paymnetHistories.length ? (
                        user.paymnetHistories.map((item) => (
                            <PaymentHistory
                                amount={item.amount}
                                createdAt={item.createdAt}
                                productPaymentHistory={item.products}
                            />
                        ))
                    ) : (
                        <h3 className="user__payment-history_descr text-center">
                            You don't have yet payment histories
                        </h3>
                    )}
                </div>
            </section>
            {user.role == roleEnum.Admin ? <AdminPanel /> : ''}
        </>
    );
};

export default Account;
