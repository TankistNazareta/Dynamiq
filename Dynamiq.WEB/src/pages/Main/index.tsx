import Card from '../../components/Card';

import range1Card from '../../assets/images/main_range_1card.png';
import range2Card from '../../assets/images/main_range_2card.png';
import range3Card from '../../assets/images/main_range_3card.png';
import funiroFurniture from '../../assets/images/funiroFurnitureMain.png';

const Main = () => {
    return (
        <main className="main">
            <section className="main__first-section">
                <div className="container d-flex justify-content-end align-items-center main__first-section__container">
                    <div className="main__banner">
                        <p className="main__banner-subtitle">New Arrival</p>
                        <h1 className="main__banner-title">
                            Discover Our <br /> New Collection
                        </h1>
                        <p className="main__banner-descr">
                            Lorem ipsum dolor sit amet, consectetur adipiscing elit. <br /> Ut elit tellus, luctus nec
                            ullamcorper mattis.
                        </p>
                        <button className="main__banner-btn">BUY Now</button>
                    </div>
                </div>
            </section>
            <section className="main__second-section d-flex flex-column justify-content-center">
                <h2 className="main__second-section-title">Browse The Range</h2>
                <p className="main__second-section-subtitle">
                    Lorem ipsum dolor sit amet, consectetur adipiscing elit.
                </p>
                <div className="main__wrapper-range d-flex flex-wrap justify-content-center">
                    <a href="" className="main__range">
                        <img src={range1Card} alt="Dining" />
                        <p>Dining</p>
                    </a>
                    <a href="" className="main__range">
                        <img src={range2Card} alt="Living" />
                        <p>Living</p>
                    </a>
                    <a href="" className="main__range">
                        <img src={range3Card} alt="Bedroom" />
                        <p>Bedroom</p>
                    </a>
                </div>
            </section>
            <section className="main__third-section">
                <h2 className="main__third-section-title">Our Products</h2>
                <div className="container main__cards">
                    <div className="d-flex flex-wrap justify-content-between gx-4 gy-4">
                        <Card additionalClasses="main__card" />
                        <Card additionalClasses="main__card" />
                        <Card additionalClasses="main__card" />
                        <Card additionalClasses="main__card" />
                        <Card additionalClasses="main__card" />
                        <Card additionalClasses="main__card" />
                        <Card additionalClasses="main__card" />
                        <Card additionalClasses="main__card" />
                    </div>
                </div>
                <button className="btn-show-more">Show more</button>
            </section>
            <section className="main__forth-section">
                <h3 className="main__forth-section-subtitle">Share your setup with</h3>
                <h2 className="main__forth-section-title">#FuniroFurniture</h2>
                <div className="main__forth-section__background-box">
                    <img className="main__forth-section-background" src={funiroFurniture} alt="" />
                </div>
            </section>
        </main>
    );
};

export default Main;
