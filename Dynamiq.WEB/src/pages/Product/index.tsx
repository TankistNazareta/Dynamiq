import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';

import Carousel from './Carousel';
import Card from '../../components/Card/Card';
import NotFound from '../NotFound';
import Loading from '../../components/Loading';

import descr1 from '../../assets/images/testCarousel/descrProduct1.png';
import descr2 from '../../assets/images/testCarousel/descrProduct2.png';

import { getByIdProduct, ProductResBody } from '../../services/client/product';
import useHttpHook from '../../hooks/useHttp';
import { ErrorMsgType } from '../../utils/types/api';
import CardList from '../../components/Card/CardList';
import { getCategoryWithParentById, GetCategoryWithParentByIdRes } from '../../services/client/category';
import { addItemToCart } from '../../services/client/cart';
import getUserIdFromAccessToken from '../../utils/services/getUserIdFromAccessToken';
import { createCheckout, CreateCheckoutType } from '../../services/client/payment';

const Product = () => {
    const { id } = useParams<{ id: string }>();

    const [error, setError] = useState<ErrorMsgType>();
    const [quantity, setQuantity] = useState(1);
    const [productData, setProductData] = useState<ProductResBody>();

    const { state, setState, makeRequest } = useHttpHook();

    const [categories, setCategories] = useState<string[]>([]);

    useEffect(() => {
        if (id === productData?.id) return;
        if (!id) {
            setError({
                StatusCode: 404,
                Message: "Product wasn't found by his id (please write id)",
            });

            setState('error');
            return;
        }

        makeRequest<ProductResBody>(() => getByIdProduct(id))
            .then((res) => {
                setProductData(res);

                makeRequest<GetCategoryWithParentByIdRes>(() => getCategoryWithParentById(res.categoryId))
                    .then((res: GetCategoryWithParentByIdRes) => {
                        setCategories([...res.categoriesSlug]);
                    })
                    .catch((err: ErrorMsgType) => {
                        setError(err);
                    });

                setState('success');
            })
            .catch((err: ErrorMsgType) => {
                setError(err);
            });
    }, [id]);

    const onPurchase = () => {
        const resOfToken = getUserIdFromAccessToken();

        if (resOfToken.error !== undefined) {
            setError({ Message: resOfToken.error, StatusCode: 401 });
            return;
        }

        makeRequest<CreateCheckoutType>(() =>
            createCheckout({
                intervalEnum: productData!.interval,
                userId: resOfToken.userId,
                productId: productData!.id,
                quantity: quantity,
            })
        ).catch((err: ErrorMsgType) => {
            setError(err);
        });
    };

    const onAddToCart = () => {
        const resOfToken = getUserIdFromAccessToken();

        if (resOfToken.error !== undefined) {
            setError({ Message: resOfToken.error, StatusCode: 500 });
            return;
        }

        addItemToCart(resOfToken.userId, productData!.id, quantity);
    };

    if (state === 'error') {
        if (error?.StatusCode === 404) return <NotFound />;
        return <h3 className="cards_error text-danger">Error: {error?.Message ?? 'Unknown'}</h3>;
    }
    if (state === 'success' && productData) {
        return (
            <View
                product={productData}
                quantity={quantity}
                setQuantity={setQuantity}
                categories={categories}
                onAddToCart={onAddToCart}
                onPurchase={onPurchase}
            />
        );
    }

    return <Loading />;
};

const View: React.FC<{
    product: ProductResBody;
    quantity: number;
    setQuantity: (q: number) => void;
    categories: string[];
    onAddToCart: () => void;
    onPurchase: () => void;
}> = ({ product, quantity, setQuantity, categories, onAddToCart, onPurchase }) => {
    const imgs = product.imgUrls.map((url) => <img key={url.imgUrl} src={url.imgUrl} alt="" />);
    const sortedDescr = product.paragraphs.sort((a, b) => b.order - a.order);

    return (
        <>
            <div className="product__subheader">
                <nav className="d-flex align-items-center container">
                    <ul className="d-flex">
                        <Link to={'/home'} className="d-flex align-items-center">
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
                        </Link>

                        <Link to={'/shop'} className="d-flex align-items-center">
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
                        </Link>

                        <hr className="hr-separetor product__subheader__hr" />
                        <li className="product__subheader__item product__subheader__item-product-name">
                            Name of product
                        </li>
                    </ul>
                </nav>
            </div>
            <section>
                <div className="container product__container d-flex">
                    <Carousel showSlides={1} imgs={imgs.length > 5 ? imgs.slice(0, 5) : imgs}>
                        {imgs.slice(0, 5).map((img, i) => (
                            <div key={i} className="carousel-comp__slide">
                                {img}
                            </div>
                        ))}
                    </Carousel>
                    <div className="product__info">
                        <h2 className="product__info_title">{product.name}</h2>
                        <h4 className="product__info_price">${product.price}</h4>
                        <p className="product__info_descr">{product.description}</p>
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
                            <button className="product__info_btn" onClick={() => onAddToCart()}>
                                Add To Cart
                            </button>
                            <button className="product__info_btn" onClick={() => onPurchase()}>
                                purchase immediately
                            </button>
                        </div>
                        <hr className="hr-separetor product__info_hr" />
                        <div className="product__info__additional">
                            <div className="product__info__additional_item d-flex justify-content-center">
                                <p className="product__info__additional_item-title">Category</p>
                                <p className="product__info__additional_item-separator">:</p>
                                <p className="product__info__additional_item-descr">{categories.join(', ')}</p>
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
                    {sortedDescr.map((item, i) => (
                        <p key={i}>{item.text}</p>
                    ))}
                    <div className="product__description_photo_wrapper d-flex flex-wrap justify-content-center">
                        {product.imgUrls.length > 5 && (
                            <div className="product__description_photo_wrapper d-flex flex-wrap justify-content-center">
                                {product.imgUrls.slice(5).map((imgUrl, index) => (
                                    <div key={index} className="product__description_photo">
                                        <img src={imgUrl.imgUrl} alt="" />
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>
                </div>
                <div className="product__description_photos d-flex flex-wrap"></div>
            </section>
            <hr className="hr-separator" />
            <section className="product__related">
                <h2 className="product__related_title">Related Products</h2>
                <div className="container product__related_cards">
                    <CardList limit={4} offset={0} productFilter={{ categoryIds: [product.categoryId] }} />
                </div>
                <button className="btn-show-more">Show more</button>
            </section>
        </>
    );
};

export default Product;
