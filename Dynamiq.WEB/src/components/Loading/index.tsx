import './loading.scss';

interface LoadingTypes {
    className?: string;
}

const Loading: React.FC<LoadingTypes> = ({ className }) => {
    return (
        <div className={`loading ${className}`}>
            <div className="spinner-border" role="status"></div>
        </div>
    );
};

export default Loading;
