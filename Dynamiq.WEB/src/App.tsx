import Header from './components/Header';
import Main from './pages/Main';
import Footer from './components/Footer';
import Shop from './pages/Shop';
import SubheaderNav from './components/SubheaderNav';
import Product from './pages/Product';
import Cart from './pages/Cart';

import './assets/scss/app.scss';

import testCarousel1 from './assets/images/testCarousel/1.png';
import testCarousel2 from './assets/images/testCarousel/2.png';
import testCarousel3 from './assets/images/testCarousel/3.png';
import testCarousel4 from './assets/images/testCarousel/4.png';
import testCarousel5 from './assets/images/testCarousel/5.png';
import Contact from './pages/Contact';
import Account from './pages/Account';
import roleEnum from './utils/enums/roleEnum';
import { Route, BrowserRouter as Router, Routes } from 'react-router-dom';
import NotFound from './pages/NotFound';
import AuthPage from './pages/Auth';
import ConfirmEmail from './pages/ConfirmEmail';
import OfflinePage from './pages/OfflinePage';
import { AccessTokenReturnType, refreshTheAccessToken } from './services/client/auth';
import { useEffect, useState } from 'react';
import useHttpHook from './hooks/useHttp';
import AuthCallBack from './pages/Auth/AuthCallback';

function App() {
    const [isAuth, setIsAuth] = useState(Boolean(localStorage.getItem('token')));

    const { makeRequest } = useHttpHook();

    useEffect(() => {
        setTimerForRefreshTheAccessToken();
    }, [isAuth]);

    const setTimerForRefreshTheAccessToken = async () => {
        if (!isAuth) return;

        let callInMs: number = 0;

        const token = localStorage.getItem('token');

        if (token != null) {
            const payloadBase64 = token!.split('.')[1];
            const payloadJson = atob(payloadBase64);
            const payload = JSON.parse(payloadJson);

            const exp = payload.exp;
            // const expiresInMs = exp ? exp * 1000 - (Date.now() + 5 * 60 * 1000) : 0;
            callInMs = exp ? exp * 1000 - Date.now() : 0;
        }

        console.log(token);

        console.log(callInMs, 'setTimerForRefreshTheAccessToken.expiresInMs ');

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
                        <Route
                            path="/product"
                            element={
                                <Product
                                    imgUrls={[
                                        testCarousel1,
                                        testCarousel2,
                                        testCarousel3,
                                        testCarousel4,
                                        testCarousel5,
                                    ]}
                                />
                            }
                        />
                        <Route path="/contact" element={<Contact />} />
                        <Route path="/cart" element={<Cart discount={100} />} />
                        <Route
                            path="/account"
                            element={
                                <Account role={roleEnum.Admin} email={'youtopak@gmail.com'} hasSubscription={false} />
                            }
                        />
                        <Route path="*" element={<NotFound />} />
                    </>
                )}
                <Route path="confirm-email/:token" element={<ConfirmEmail />} />
                <Route path="error" element={<OfflinePage />} />
                <Route path="auth/callback" element={<AuthCallBack onLogIn={onLogIn} />} />
            </Routes>

            {isAuth && <Footer />}
        </>
    );
}

export default App;
