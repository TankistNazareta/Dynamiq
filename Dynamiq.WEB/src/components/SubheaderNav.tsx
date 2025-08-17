import '../assets/scss/app.scss';

interface SubheaderNavProps {
    nameRoute: string;
}

const SubheaderNav: React.FC<SubheaderNavProps> = ({ nameRoute }) => {
    return (
        <section className="subheader-nav">
            <div className="subheader-nav__wrapper">
                <h2 className="subheader-nav__title">{nameRoute}</h2>
                <nav>
                    <ul className="subheader-nav__list d-flex">
                        <li className="subheader-nav__list__item">Home</li>
                        <li className="subheader-nav__list__item">{'>'}</li>
                        <li className="subheader-nav__list__item subheader-nav__list__item-active">{nameRoute}</li>
                    </ul>
                </nav>
            </div>
        </section>
    );
};

export default SubheaderNav;
