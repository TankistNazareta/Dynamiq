import { useState } from 'react';

import Carousel from './Carousel';

import descr1 from '../../assets/images/testCarousel/descrProduct1.png';
import descr2 from '../../assets/images/testCarousel/descrProduct2.png';
import Card from '../../components/Card';

interface ProductProps {
    imgUrls: string[];
}

const Product: React.FC<ProductProps> = ({ imgUrls }) => {
    const [quantity, setQuantity] = useState(1);

    var imgs = imgUrls.map((url) => {
        return <img src={url} alt="" />;
    });

    return (
        <>
            <div className="product__subheader">
                <nav className="d-flex align-items-center container">
                    <ul className="d-flex">
                        <li className="product__subheader__item">Home</li>
                        <li className="product__subheader__item product__subheader__item-arrow">
                            <svg
                                width="8"
                                height="14"
                                viewBox="0 0 8 14"
                                fill="none"
                                xmlns="http://www.w3.org/2000/svg">
                                <path d="M0 12L5 7L0 2L1 0L8 7L1 14L0 12Z" fill="black" />
                            </svg>
                        </li>
                        <li className="product__subheader__item">Shop</li>
                        <li className="product__subheader__item product__subheader__item-arrow">
                            <svg
                                width="8"
                                height="14"
                                viewBox="0 0 8 14"
                                fill="none"
                                xmlns="http://www.w3.org/2000/svg">
                                <path d="M0 12L5 7L0 2L1 0L8 7L1 14L0 12Z" fill="black" />
                            </svg>
                        </li>
                        <hr className="hr-separetor product__subheader__hr" />
                        <li className="product__subheader__item product__subheader__item-product-name">
                            Name of product
                        </li>
                    </ul>
                </nav>
            </div>
            <section>
                <div className="container product__container d-flex">
                    <Carousel showSlides={1} imgs={imgs}>
                        {imgs.map((img) => (
                            <div className="carousel-comp__slide">{img}</div>
                        ))}
                    </Carousel>
                    <div className="product__info">
                        <h2 className="product__info_title">Asgaard sofa</h2>
                        <h4 className="product__info_price">Rs. 250,000.00</h4>
                        <p className="product__info_descr">
                            Setting the bar as one of the loudest speakers in its class, the Kilburn is a compact,
                            stout-hearted hero with a well-balanced audio which boasts a clear midrange and extended
                            highs for a sound.
                        </p>
                        <div className="product__info_btns d-flex flex-wrap justify-content-between align-items-center">
                            <div className="product__info_counter d-flex">
                                <button
                                    onClick={() => {
                                        if (quantity !== 0) setQuantity(quantity - 1);
                                    }}
                                    className="product__info_counter-btn product__info_counter-btn-remove">
                                    -
                                </button>
                                <input
                                    value={quantity || 0}
                                    type="string"
                                    className="product__info_counter-input"
                                    onChange={(e) => {
                                        const onlyNums = e.target.value.replace(/\D/g, '');
                                        setQuantity(Number.parseInt(onlyNums));
                                    }}
                                />
                                <button
                                    onClick={() => setQuantity(quantity + 1)}
                                    className="product__info_counter-btn product__info_counter-btn-add">
                                    +
                                </button>
                            </div>
                            <button className="product__info_btn">Add To Cart</button>
                            <button className="product__info_btn">purshade immidiatly</button>
                        </div>
                        <hr className="hr-separetor product__info_hr" />
                        <div className="product__info__additional">
                            <div className="product__info__additional_item d-flex justify-content-center">
                                <p className="product__info__additional_item-title">Category</p>
                                <p className="product__info__additional_item-separator">:</p>
                                <p className="product__info__additional_item-descr">Sofas</p>
                            </div>
                            <div className="product__info__additional_item d-flex justify-content-center">
                                <p className="product__info__additional_item-title">Share</p>
                                <p className="product__info__additional_item-separator">:</p>
                                <a className="product__info__additional_item-descr product__info__additional_item-link">
                                    Click
                                </a>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
            <hr className="hr-separator" />
            <section className="product__description container">
                <h2 className="product__description_title">Description</h2>
                <div className="product__description__paragraphs">
                    <p>
                        Embodying the raw, wayward spirit of rock ‘n’ roll, the Kilburn portable active stereo speaker
                        takes the unmistakable look and sound of Marshall, unplugs the chords, and takes the show on the
                        road.
                    </p>
                    <p>
                        Weighing in under 7 pounds, the Kilburn is a lightweight piece of vintage styled engineering.
                        Setting the bar as one of the loudest speakers in its class, the Kilburn is a compact,
                        stout-hearted hero with a well-balanced audio which boasts a clear midrange and extended highs
                        for a sound that is both articulate and pronounced. The analogue knobs allow you to fine tune
                        the controls to your personal preferences while the guitar-influenced leather strap enables easy
                        and stylish travel.
                    </p>
                    <div className="product__description_photo_wrapper d-flex flex-wrap justify-content-center">
                        <div className="product__description_photo">
                            <img src={descr1} alt="" />
                        </div>
                        <div className="product__description_photo">
                            <img src={descr2} alt="" />
                        </div>
                    </div>
                </div>
                <div className="product__description_photos d-flex flex-wrap"></div>
            </section>
            <hr className="hr-separator" />
            <section className="product__realted">
                <h2 className="product__realted_title">Related Products</h2>
                <div className="container product__realted_cards">
                    <div className="d-flex flex-wrap justify-content-between gx-4 gy-4">
                        <Card additionalClasses="main__card" />
                        <Card additionalClasses="main__card" />
                        <Card additionalClasses="main__card" />
                        <Card additionalClasses="main__card" />
                    </div>
                </div>
                <button className="btn-show-more">Show more</button>
            </section>
        </>
    );
};

export default Product;
