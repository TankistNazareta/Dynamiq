const Footer = () => {
    return (
        <>
            <hr className="footer__first-line hr-separetor" />
            <footer className="footer">
                <div className="footer__wrapper container">
                    <div className="row">
                        <div className="col-lg-5 col-6 footer__colum-logo">
                            <div className="footer__logo">
                                <h3 className="footer__logo-title">Dynamiq</h3>
                                <p className="footer__logo-descr">
                                    400 University Drive Suite 200 Coral Gables, <br />
                                    FL 33134 USA
                                </p>
                            </div>
                        </div>
                        <div className="col-3 footer__colum-links">
                            <nav className="footer__nav">
                                <h4 className="footer__title">Links</h4>
                                <ul className="footer__list">
                                    <li className="footer__item">Home</li>
                                    <li className="footer__item">Shop</li>
                                    <li className="footer__item">About</li>
                                    <li className="footer__item">Contact</li>
                                </ul>
                            </nav>
                        </div>
                        <div className="col-3 footer__colum-links">
                            <div className="footer__socials d-flex flex-column">
                                <h4 className="footer__title">Social</h4>
                                <a className="footer__item footer__social-link" href="">
                                    Facebook
                                </a>
                                <a className="footer__item footer__social-link" href="">
                                    GitHub
                                </a>
                                <a className="footer__item footer__social-link" href="">
                                    X
                                </a>
                            </div>
                        </div>
                    </div>
                </div>
                <hr className="footer__end-line" />
                <div className="footer__bottom">
                    <p>&copy; 2025 Dynamiq. All rights reserved.</p>
                </div>
            </footer>
        </>
    );
};

export default Footer;
