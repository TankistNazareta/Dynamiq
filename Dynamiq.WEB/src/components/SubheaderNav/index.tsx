import './subheaderNav.scss';

import { Link } from 'react-router-dom';

interface SubheaderNavProps {
    nameRoute: string;
}

const SubheaderNav: React.FC<SubheaderNavProps> = ({ nameRoute }) => {
    return (
        <section className="subheader-nav">
            <div className="subheader-nav__wrapper">
                <h2 className="subheader-nav__title">{nameRoute}</h2>
                <nav>
                    <ul className="subheader-nav__list">
                        <Link to="/" className="subheader-nav__list-item">
                            Home
                        </Link>
                        <li className="subheader-nav__list-item">{'>'}</li>
                        <Link className="subheader-nav__list-item subheader-nav__list-item-active" to={'/' + nameRoute}>
                            {nameRoute}
                        </Link>
                    </ul>
                </nav>
            </div>
        </section>
    );
};

export default SubheaderNav;
