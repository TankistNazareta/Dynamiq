import Header from './components/Header';
import Main from './pages/Main';
import Footer from './components/Footer';
import Shop from './pages/Shop';
import Product from './pages/Product';
import Cart from './pages/Cart';

import Contact from './pages/Contact';
import Account from './pages/Account';
import { Route, BrowserRouter as Router, Routes } from 'react-router-dom';
import NotFound from './pages/NotFound';
import AuthPage from './pages/Auth';
import ConfirmEmail from './pages/ConfirmEmail';
import OfflinePage from './pages/OfflinePage';
import { AccessTokenReturnType, refreshTheAccessToken } from './services/client/auth';
import { useEffect, useState } from 'react';
import useHttpHook from './hooks/useHttp';
import AuthCallBack from './pages/Auth/AuthCallback';
import InfoMsg from './components/InfoMsg';
import PaymentStatus from './pages/PaymentStatus';
import About from './pages/About';

const AppInner = () => {
    const [isAuth, setIsAuth] = useState(true);

    const { makeRequest } = useHttpHook();

    useEffect(() => {
        setTimerForRefreshTheAccessToken();
    }, []);

    useEffect(() => {
        if (localStorage.getItem('token') === undefined) setIsAuth(false);
    }, [localStorage.getItem('token')]);

    const setTimerForRefreshTheAccessToken = async () => {
        if (!isAuth) return;

        let callInMs: number = 0;

        const token = localStorage.getItem('token');

        if (token != null) {
            const payloadBase64 = token!.split('.')[1];
            const payloadJson = atob(payloadBase64);
            const payload = JSON.parse(payloadJson);

            const exp = payload.exp;
            callInMs = exp ? exp * 1000 - (Date.now() + 5 * 60 * 1000) : 0;
        }

        setTimeout(async () => {
            makeRequest<AccessTokenReturnType>(() => refreshTheAccessToken())
                .then(() => {
                    setTimerForRefreshTheAccessToken();
                })
                .catch(() => setIsAuth(false));
        }, callInMs);
    };

    const onLogIn = () => {
        setIsAuth(true);
    };

    return (
        <>
            {isAuth && <Header />}
            <Routes>
                {!isAuth ? (
                    <Route path="*" element={<AuthPage onLogIn={onLogIn} />} />
                ) : (
                    <>
                        <Route path="/" element={<Main />} />
                        <Route path="/shop" element={<Shop />} />
                        <Route path="/product/:id" element={<Product />} />
                        <Route path="/contact" element={<Contact />} />
                        <Route path="/cart" element={<Cart />} />
                        <Route path="/about" element={<About />} />
                        <Route path="/account" element={<Account />} />
                        <Route path="*" element={<NotFound />} />
                        <Route path="payment/success" element={<PaymentStatus status={'success'} />} />
                        <Route path="payment/failed" element={<PaymentStatus status={'failed'} />} />
                    </>
                )}
                <Route path="confirm-email/:token" element={<ConfirmEmail />} />
                <Route path="error" element={<OfflinePage />} />
                <Route path="auth/callback" element={<AuthCallBack onLogIn={onLogIn} />} />
            </Routes>
            <InfoMsg />
            {isAuth && <Footer />}
        </>
    );
};

export default AppInner;
