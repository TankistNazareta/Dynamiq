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

function App() {
    return (
        <>
            <Header />
            {/* <Main /> */}
            {/* <SubheaderNav nameRoute="Shop" /> */}
            {/* <Shop /> */}
            {/* <Product imgUrls={[testCarousel1, testCarousel2, testCarousel3, testCarousel4, testCarousel5]} /> */}
            <SubheaderNav nameRoute="Cart" />
            <Cart discount={100} />
            <Footer />
        </>
    );
}

export default App;
