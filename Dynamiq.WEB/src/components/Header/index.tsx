import { useEffect, useRef, useState } from 'react';

import logo from '../../assets/images/loge.png';
import PopupCart from './Cart/PopupCart';
import { useLocation, useNavigate, Link, useSearchParams } from 'react-router-dom';
import { CloseButton } from 'react-bootstrap';
import useHttpHook from '../../hooks/useHttp';
import { getSearchedNames, SearchedNamesRes } from '../../services/client/product';
import { CategoryRes, getAllCategories } from '../../services/client/category';
import getFilterFromUrl from '../../utils/services/getFilterFromUrl';

const Header = () => {
    const [needSideBar, setNeedSideBar] = useState<boolean>(false);
    const [needToShowCart, setNeedToShowCart] = useState(false);
    const [needToShowSearch, setNeedToShowSearch] = useState(false);
    const [searchInput, setSearchInput] = useState('');
    const [categoryItems, setCategoryItems] = useState<CategoryRes[]>([]);
    const [suggestionNames, setSuggestionNames] = useState<string[]>([]);

    const sidebarRef = useRef<HTMLDivElement>(null);
    const hamburgerRef = useRef<HTMLDivElement>(null);

    const location = useLocation();
    const navigate = useNavigate();
    const [searchParams, setSearchParams] = useSearchParams();

    const { setState, makeRequest } = useHttpHook();

    useEffect(() => {
        if (!categoryItems.length) {
            makeRequest<CategoryRes[]>(() => getAllCategories())
                .then((res: CategoryRes[]) => {
                    setCategoryItems(res);
                })
                .then(() => setState('success'));
        }
    }, []);

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent | TouchEvent) => {
            if (
                sidebarRef.current &&
                !sidebarRef.current.contains(event.target as Node) &&
                hamburgerRef.current &&
                !hamburgerRef.current.contains(event.target as Node)
            ) {
                setNeedSideBar(false);
            }
        };

        document.body.addEventListener('click', handleClickOutside);
        document.body.addEventListener('touchstart', handleClickOutside);

        return () => {
            document.body.removeEventListener('click', handleClickOutside);
            document.body.removeEventListener('touchstart', handleClickOutside);
        };
    }, []);

    useEffect(() => {
        if (searchInput.length >= 2) {
            const filter = getFilterFromUrl(searchParams, categoryItems);

            filter.searchTerm = searchInput;

            makeRequest<SearchedNamesRes>(() => getSearchedNames(filter)).then((data) =>
                setSuggestionNames(data.names)
            );
        } else if (suggestionNames.length !== 0) {
            setSuggestionNames([]);
        }
    }, [searchInput]);

    const onSearch = (search: string) => {
        addSearchHistrory(search);

        if (location.pathname.startsWith('/shop')) {
            searchParams.set('search', search);
            if (!search.trim()) searchParams.delete('search');

            setSearchParams(searchParams);
        } else if (search.trim()) {
            navigate(`/shop?search=${encodeURIComponent(search)}`);
        } else {
            navigate('/shop');
        }

        setSearchInput('');
        setNeedToShowSearch(false);
    };

    const getSearchHistory = () => {
        const searchHistoryJson = localStorage.getItem('searchHistory');

        return searchHistoryJson === null ? [] : (JSON.parse(searchHistoryJson) as string[]);
    };

    const addSearchHistrory = (searchInput: string) => {
        let history = getSearchHistory().filter((item) => item !== searchInput);

        history.unshift(searchInput);

        localStorage.setItem('searchHistory', JSON.stringify(history));
    };

    const searchHistory = getSearchHistory();

    return (
        <>
            <header className="header">
                <div className="header__wrapper d-flex justify-content-around align-items-center">
                    <a className="header__logo d-flex align-items-center justify-content-center" href="/">
                        <img className="header__logo-image" src={logo} alt="logo" />
                        <h3 className="header__logo-title">Dynamiq</h3>
                    </a>
                    <nav className="header__nav">
                        <ul className="header__list d-flex justify-content-between">
                            <li className="header__item">
                                <Link to="/">Home</Link>
                            </li>
                            <li className="header__item">
                                <Link to="/shop">Shop</Link>
                            </li>
                            <li className="header__item">
                                <Link to="/about">About</Link>
                            </li>
                            <li className="header__item">
                                <Link to="/contact">Contact</Link>
                            </li>
                        </ul>
                    </nav>
                    <div className="d-flex justify-content-between align-items-center header__socials">
                        <Link to="/account">
                            <button className="header__social-link">
                                <svg
                                    width="28"
                                    height="28"
                                    viewBox="0 0 28 28"
                                    fill="none"
                                    xmlns="http://www.w3.org/2000/svg">
                                    <path
                                        d="M23.3333 14V8.16669H25.6666V15.1667H23.3333M23.3333 19.8334H25.6666V17.5H23.3333M11.6666 15.1667C14.7816 15.1667 21 16.73 21 19.8334V23.3334H2.33331V19.8334C2.33331 16.73 8.55165 15.1667 11.6666 15.1667ZM11.6666 4.66669C12.9043 4.66669 14.0913 5.15835 14.9665 6.03352C15.8416 6.90869 16.3333 8.09568 16.3333 9.33335C16.3333 10.571 15.8416 11.758 14.9665 12.6332C14.0913 13.5084 12.9043 14 11.6666 14C10.429 14 9.24198 13.5084 8.36682 12.6332C7.49164 11.758 6.99998 10.571 6.99998 9.33335C6.99998 8.09568 7.49164 6.90869 8.36682 6.03352C9.24198 5.15835 10.429 4.66669 11.6666 4.66669ZM11.6666 17.3834C8.20165 17.3834 4.54998 19.0867 4.54998 19.8334V21.1167H18.7833V19.8334C18.7833 19.0867 15.1316 17.3834 11.6666 17.3834ZM11.6666 6.88335C11.0169 6.88335 10.3937 7.14148 9.93424 7.60094C9.47477 8.06041 9.21665 8.68357 9.21665 9.33335C9.21665 9.98313 9.47477 10.6063 9.93424 11.0658C10.3937 11.5252 11.0169 11.7834 11.6666 11.7834C12.3164 11.7834 12.9396 11.5252 13.3991 11.0658C13.8585 10.6063 14.1166 9.98313 14.1166 9.33335C14.1166 8.68357 13.8585 8.06041 13.3991 7.60094C12.9396 7.14148 12.3164 6.88335 11.6666 6.88335Z"
                                        fill="black"
                                    />
                                </svg>
                                <span className="btn_descr">Profile</span>
                            </button>
                        </Link>
                        <button className="header__social-link" onClick={() => setNeedToShowSearch(true)}>
                            <svg
                                width="28"
                                height="28"
                                viewBox="0 0 28 28"
                                fill="none"
                                xmlns="http://www.w3.org/2000/svg">
                                <path
                                    d="M24.5 24.5L19.2663 19.257M22.1666 12.25C22.1666 14.88 21.1219 17.4024 19.2621 19.2621C17.4024 21.1219 14.88 22.1666 12.25 22.1666C9.61992 22.1666 7.09757 21.1219 5.23784 19.2621C3.3781 17.4024 2.33331 14.88 2.33331 12.25C2.33331 9.61992 3.3781 7.09757 5.23784 5.23784C7.09757 3.3781 9.61992 2.33331 12.25 2.33331C14.88 2.33331 17.4024 3.3781 19.2621 5.23784C21.1219 7.09757 22.1666 9.61992 22.1666 12.25V12.25Z"
                                    stroke="black"
                                    stroke-width="2"
                                    stroke-linecap="round"
                                />
                            </svg>
                            <span className="btn_descr">Search</span>
                        </button>
                        <div className="header__social-link-cart">
                            <button className="header__social-link" onClick={() => setNeedToShowCart(true)}>
                                <svg
                                    width="28"
                                    height="28"
                                    viewBox="0 0 28 28"
                                    fill="none"
                                    xmlns="http://www.w3.org/2000/svg">
                                    <path
                                        d="M25.5621 18.6379H9.27891L10.0965 16.9727L23.6809 16.948C24.1402 16.948 24.534 16.6199 24.616 16.166L26.4973 5.63594C26.5465 5.35977 26.4727 5.07539 26.2922 4.85938C26.203 4.75306 26.0917 4.66743 25.9661 4.60841C25.8405 4.54939 25.7036 4.51839 25.5648 4.51758L8.28359 4.46016L8.13594 3.76563C8.04297 3.32266 7.64375 3 7.18984 3H2.96523C2.70924 3 2.46373 3.10169 2.28271 3.28271C2.10169 3.46373 2 3.70924 2 3.96523C2 4.22123 2.10169 4.46674 2.28271 4.64776C2.46373 4.82877 2.70924 4.93047 2.96523 4.93047H6.40781L7.05312 7.99844L8.6418 15.6902L6.59648 19.0289C6.49027 19.1723 6.42629 19.3425 6.4118 19.5203C6.3973 19.6981 6.43286 19.8765 6.51445 20.0352C6.67852 20.3605 7.00937 20.5656 7.37578 20.5656H9.09297C8.72689 21.0519 8.52915 21.6441 8.52969 22.2527C8.52969 23.8004 9.7875 25.0582 11.3352 25.0582C12.8828 25.0582 14.1406 23.8004 14.1406 22.2527C14.1406 21.643 13.9383 21.0496 13.5773 20.5656H17.9824C17.6163 21.0519 17.4186 21.6441 17.4191 22.2527C17.4191 23.8004 18.677 25.0582 20.2246 25.0582C21.7723 25.0582 23.0301 23.8004 23.0301 22.2527C23.0301 21.643 22.8277 21.0496 22.4668 20.5656H25.5648C26.0953 20.5656 26.5301 20.1336 26.5301 19.6004C26.5285 19.3447 26.4258 19.0999 26.2445 18.9196C26.0631 18.7393 25.8178 18.638 25.5621 18.6379ZM8.68555 6.36328L24.4301 6.41523L22.8879 15.0504L10.5203 15.0723L8.68555 6.36328ZM11.3352 23.1168C10.8594 23.1168 10.4711 22.7285 10.4711 22.2527C10.4711 21.777 10.8594 21.3887 11.3352 21.3887C11.8109 21.3887 12.1992 21.777 12.1992 22.2527C12.1992 22.4819 12.1082 22.7017 11.9461 22.8637C11.7841 23.0258 11.5643 23.1168 11.3352 23.1168ZM20.2246 23.1168C19.7488 23.1168 19.3605 22.7285 19.3605 22.2527C19.3605 21.777 19.7488 21.3887 20.2246 21.3887C20.7004 21.3887 21.0887 21.777 21.0887 22.2527C21.0887 22.4819 20.9976 22.7017 20.8356 22.8637C20.6735 23.0258 20.4538 23.1168 20.2246 23.1168Z"
                                        fill="black"
                                    />
                                </svg>
                                <span className="btn_descr">Cart</span>
                            </button>
                            {needToShowCart && (
                                <PopupCart
                                    needToShow={needToShowCart}
                                    setNeedToShowToFalse={() => setNeedToShowCart(false)}
                                />
                            )}
                        </div>

                        <div
                            ref={hamburgerRef}
                            className={`header__hamburger ${needSideBar ? 'header__hamburger-active' : ''}`}
                            onClick={() => setNeedSideBar((needSideBar) => !needSideBar)}>
                            <span></span>
                            <span></span>
                            <span></span>
                        </div>
                    </div>
                </div>
                <aside ref={sidebarRef} className={`header__sidebar ${needSideBar ? 'header__sidebar-active' : ''}`}>
                    <nav className="header__nav header__sidebar-nav">
                        <ul className="header__list header__sidebar-list d-flex flex-column justify-content-between">
                            <li className="header__item header__sidebar-item">
                                <a href="/">Home</a>
                            </li>
                            <hr className="header__sidebar-line" />
                            <li className="header__item header__sidebar-item">
                                <a href="/shop">Shop</a>
                            </li>
                            <hr className="header__sidebar-line" />
                            <li className="header__item header__sidebar-item">
                                <a href="/about">About</a>
                            </li>
                            <hr className="header__sidebar-line" />
                            <li className="header__item header__sidebar-item">
                                <a href="contact">Contact</a>
                            </li>
                        </ul>
                    </nav>
                </aside>
            </header>
            <div
                className={`header__search d-flex justify-content-center align-items-center ${
                    needToShowSearch ? 'header__search-active' : 'header__search-not_active'
                }`}>
                <div className="container">
                    <form
                        className="header__search__inner"
                        onSubmit={(e) => {
                            e.preventDefault();
                            onSearch(searchInput);
                        }}>
                        <button className="header__search__btn header__search__btn-search">
                            <svg
                                width="28"
                                height="28"
                                viewBox="0 0 28 28"
                                fill="none"
                                xmlns="http://www.w3.org/2000/svg">
                                <path
                                    d="M24.5 24.5L19.2663 19.257M22.1666 12.25C22.1666 14.88 21.1219 17.4024 19.2621 19.2621C17.4024 21.1219 14.88 22.1666 12.25 22.1666C9.61992 22.1666 7.09757 21.1219 5.23784 19.2621C3.3781 17.4024 2.33331 14.88 2.33331 12.25C2.33331 9.61992 3.3781 7.09757 5.23784 5.23784C7.09757 3.3781 9.61992 2.33331 12.25 2.33331C14.88 2.33331 17.4024 3.3781 19.2621 5.23784C21.1219 7.09757 22.1666 9.61992 22.1666 12.25V12.25Z"
                                    stroke="black"
                                    stroke-width="2"
                                    stroke-linecap="round"
                                />
                            </svg>
                        </button>
                        <input
                            value={searchInput}
                            onChange={(e) => setSearchInput(e.target.value)}
                            type="text"
                            className="header__search__input"
                        />
                        <CloseButton
                            onClick={() => setNeedToShowSearch(false)}
                            aria-label="Close"
                            className="header__search__btn header__search__btn-close"
                        />
                    </form>
                    {(suggestionNames.length !== 0 || searchHistory.length !== 0) && (
                        <div className="header__search_suggestion">
                            {(suggestionNames.length ? suggestionNames : searchHistory).map((name, i, arr) => (
                                <div key={i}>
                                    <button className="header__search_suggestion__btn" onClick={() => onSearch(name)}>
                                        {name}
                                    </button>
                                    {i !== arr.length - 1 && <hr className="hr-separator" />}
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div>
        </>
    );
};

export default Header;
