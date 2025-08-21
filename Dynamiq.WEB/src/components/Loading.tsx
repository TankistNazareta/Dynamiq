interface LoadingTypes {
    className?: string;
}

const Loading: React.FC<LoadingTypes> = ({ className }) => {
    return (
        <div className={`loading d-flex justify-content-center align-items-center ${className}`}>
            <div className="spinner-border" role="status"></div>
        </div>
    );
};

export default Loading;
