import { useState } from 'react';
import PaymentHistory from './PaymentHistory';

interface userData {
    email: string;
    emailConfirmed: boolean;
    haveSubscription: boolean;
    createdAt: Date;
    paymentHistory: [];
}

const AdminPanel = () => {
    const [inputEmail, setInputEmail] = useState<string>();
    const data: userData = {
        email: 'youtopak@gmail.com',
        emailConfirmed: true,
        haveSubscription: false,
        createdAt: new Date(),
        paymentHistory: [],
    };

    return (
        <section className="admin-panel container">
            <h4 className="admin-panel_title">Your admin panel</h4>
            <p className="admin-panel_descr">Here you can write somebody's email and you'll get his info</p>
            <form action="" className="admin-panel__form d-flex justify-content-center fle-wrap">
                <input
                    value={inputEmail}
                    onChange={(e) => setInputEmail(e.target.value)}
                    type="text"
                    className="admin-panel__form_input"
                    placeholder="Email"
                />
                <button type="submit" className="admin-panel__form_btn">
                    Submit
                </button>
            </form>
            <div className="admin-panel__result d-flex flex-wrap justify-content-around">
                {data == null || data == undefined || Object.keys(data).length === 0}
                <p className="user__payment-history_item_descr">{data.email}</p>
                <p className="user__payment-history_item_descr">
                    {'Created at: '}
                    {data.createdAt.toLocaleString('en-GB', {
                        month: 'short',
                        day: '2-digit',
                        hour: '2-digit',
                        minute: '2-digit',
                        hour12: false,
                    })}
                </p>
                <p className="user__payment-history_item_descr">email confirmed: {data.emailConfirmed ? '✔' : '❌'}</p>
                <p className="user__payment-history_item_descr">
                    has subscription: {data.haveSubscription ? '✔' : '❌'}
                </p>
                <p className="user__payment-history_item_descr admin-panel__result_payment-history">
                    Payment Histories:
                </p>
                <div className="user__payment-history__container d-flex flex-column admin-panel__result_payment-history__container container">
                    <PaymentHistory amount={100} createdAt={new Date()} />
                    <PaymentHistory amount={100} createdAt={new Date()} />
                    <PaymentHistory amount={100} createdAt={new Date()} />
                </div>
            </div>
        </section>
    );
};

export default AdminPanel;
