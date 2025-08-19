import roleEnum from '../../utils/enums/roleEnum';
import AdminPanel from './components/AdminPanel';
import PaymentHistory from './components/PaymentHistory';

interface AccountProps {
    role: roleEnum;
    email: string;
    hasSubscription: boolean;
}

const Account: React.FC<AccountProps> = ({ role, email, hasSubscription }) => {
    return (
        <>
            <section className="user__about">
                <h2 className="user__about_welcome">Welcome back!</h2>
                <h5 className="user__about_descr">
                    Your role - {role == roleEnum.Admin ? 'Admin' : 'default user'}. <br />
                    Your email - {email}. <br />
                    You
                    {hasSubscription ? ' have' : " don't have"} subscribtion <br /> We was waiting for you
                </h5>
            </section>
            <section className="user__payment-history container">
                <div className="user__payment-history__header d-flex align-items-center">Your payment history:</div>
                <div className="user__payment-history__container d-flex flex-column">
                    <PaymentHistory amount={100} createdAt={new Date()} />
                    <PaymentHistory amount={100} createdAt={new Date()} />
                    <PaymentHistory amount={100} createdAt={new Date()} />
                </div>
            </section>
            {role == roleEnum.Admin ? <AdminPanel /> : ''}
        </>
    );
};

export default Account;
