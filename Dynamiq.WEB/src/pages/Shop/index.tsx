import { useState } from 'react';
import Feature from '../../components/Feature';
import SubheaderNav from '../../components/SubheaderNav';
import { ProductFilter } from '../../services/client/product';
import CardList from '../../components/Card/CardList';
import FilterMenu from './components/FilterMenu';

const Shop = () => {
    const [dotCount, setDotCount] = useState(1);
    const [productFilter, setProductFilter] = useState<ProductFilter>();
    const [needShowFilterMenu, setNeedShowFilterMenu] = useState(false);
    const [totalCount, setTotalCount] = useState<number>(0);

    const onFilter = (filterProp: ProductFilter) => {
        setProductFilter(filterProp);
    };

    return (
        <>
            <SubheaderNav nameRoute="Shop" />
            <div className="shop">
                <div className="shop__subheader">
                    <div className="container d-flex align-items-center">
                        <button
                            className="shop__subheader-item d-flex align-items-center"
                            onClick={(e) => setNeedShowFilterMenu((prev) => !prev)}>
                            <svg
                                width="21"
                                height="18"
                                viewBox="0 0 21 18"
                                fill="none"
                                xmlns="http://www.w3.org/2000/svg">
                                <path
                                    d="M20.0238 3.14285H6.92857M4.54762 3.14285H0.976191M20.0238 15.0476H6.92857M4.54762 15.0476H0.976191M14.0714 9.09524H0.976191M20.0238 9.09524H16.4524M5.7381 0.761902C6.05383 0.761902 6.35663 0.887327 6.57989 1.11058C6.80315 1.33384 6.92857 1.63664 6.92857 1.95238V4.33333C6.92857 4.64906 6.80315 4.95187 6.57989 5.17512C6.35663 5.39838 6.05383 5.52381 5.7381 5.52381C5.42236 5.52381 5.11956 5.39838 4.8963 5.17512C4.67304 4.95187 4.54762 4.64906 4.54762 4.33333V1.95238C4.54762 1.63664 4.67304 1.33384 4.8963 1.11058C5.11956 0.887327 5.42236 0.761902 5.7381 0.761902V0.761902ZM5.7381 12.6667C6.05383 12.6667 6.35663 12.7921 6.57989 13.0153C6.80315 13.2386 6.92857 13.5414 6.92857 13.8571V16.2381C6.92857 16.5538 6.80315 16.8566 6.57989 17.0799C6.35663 17.3031 6.05383 17.4286 5.7381 17.4286C5.42236 17.4286 5.11956 17.3031 4.8963 17.0799C4.67304 16.8566 4.54762 16.5538 4.54762 16.2381V13.8571C4.54762 13.5414 4.67304 13.2386 4.8963 13.0153C5.11956 12.7921 5.42236 12.6667 5.7381 12.6667ZM15.2619 6.71428C15.5776 6.71428 15.8804 6.83971 16.1037 7.06297C16.327 7.28622 16.4524 7.58903 16.4524 7.90476V10.2857C16.4524 10.6014 16.327 10.9042 16.1037 11.1275C15.8804 11.3508 15.5776 11.4762 15.2619 11.4762C14.9462 11.4762 14.6434 11.3508 14.4201 11.1275C14.1969 10.9042 14.0714 10.6014 14.0714 10.2857V7.90476C14.0714 7.58903 14.1969 7.28622 14.4201 7.06297C14.6434 6.83971 14.9462 6.71428 15.2619 6.71428V6.71428Z"
                                    stroke="black"
                                    stroke-linecap="round"
                                    stroke-linejoin="round"
                                />
                            </svg>
                            <p className="shop__subheader_p">Filter</p>
                        </button>
                        <hr className="hr-separator shop__subheader_hr-separator" />
                        <p className="shop__subheader_p">
                            Total found: {totalCount !== null ? totalCount : 'Loading...'}
                        </p>
                    </div>
                </div>
                <section className="shop__first-section">
                    <div className="container shop__cards">
                        <CardList
                            limit={16}
                            offset={dotCount * 16 - 16}
                            productFilter={productFilter}
                            setTotalCount={setTotalCount}
                        />
                    </div>
                    <div className="shop__dots d-flex justify-content-center align-items-center">
                        {dotCount > 1 && (
                            <button
                                onClick={() => setDotCount(dotCount - 1)}
                                className="shop__dot shop__dot-additional shop__dot-prev">
                                Previos
                            </button>
                        )}

                        {dotCount > 2 && (
                            <button onClick={() => setDotCount(dotCount - 2)} className="shop__dot">
                                {dotCount - 2}
                            </button>
                        )}

                        {dotCount > 1 && (
                            <button onClick={() => setDotCount(dotCount - 1)} className="shop__dot">
                                {dotCount - 1}
                            </button>
                        )}

                        <button className="shop__dot shop__dot-active">{dotCount}</button>

                        {dotCount * 16 < totalCount && (
                            <button onClick={() => setDotCount(dotCount + 1)} className="shop__dot">
                                {dotCount + 1}
                            </button>
                        )}

                        {dotCount * 16 + 16 < totalCount && (
                            <button onClick={() => setDotCount(dotCount + 2)} className="shop__dot">
                                {dotCount + 2}
                            </button>
                        )}

                        {dotCount * 16 < totalCount && (
                            <button
                                onClick={() => setDotCount(dotCount + 1)}
                                className="shop__dot shop__dot-additional shop__dot-next">
                                Next
                            </button>
                        )}
                    </div>
                </section>
                <Feature />
            </div>
            <FilterMenu
                isActive={needShowFilterMenu}
                onFilterProp={onFilter}
                setNeedShowFilterMenu={setNeedShowFilterMenu}
            />
        </>
    );
};

export default Shop;
