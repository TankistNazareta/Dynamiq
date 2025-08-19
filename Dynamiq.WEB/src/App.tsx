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

function App() {
    return (
        <>
            <Header />

            <Router>
                <Routes>
                    <Route path="/" element={<Main />} />
                    <Route path="/shop" element={<Shop />} />
                    <Route
                        path="/product"
                        element={
                            <Product
                                imgUrls={[testCarousel1, testCarousel2, testCarousel3, testCarousel4, testCarousel5]}
                            />
                        }
                    />
                    <Route path="/contact" element={<Contact />} />
                    <Route path="/cart" element={<Cart discount={100} />} />
                    <Route
                        path="/account"
                        element={<Account role={roleEnum.Admin} email={'youtopak@gmail.com'} hasSubscription={false} />}
                    />
                    <Route path="*" element={<NotFound />} />
                </Routes>
            </Router>

            <Footer />
        </>
    );
}

export default App;
