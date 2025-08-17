import Header from './components/Header';
import Main from './pages/Main';
import Footer from './components/Footer';
import Shop from './pages/Shop';
import SubheaderNav from './components/SubheaderNav';

function App() {
    return (
        <>
            <Header />
            <SubheaderNav nameRoute="Shop" />
            {/* <Main /> */}
            <Shop />
            <Footer />
        </>
    );
}

export default App;
