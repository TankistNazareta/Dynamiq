import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Spinner, Container, Row, Col } from 'react-bootstrap';

type AuthCallBackProps = {
    onLogIn: Function;
};

const AuthCallBack: React.FC<AuthCallBackProps> = ({ onLogIn }) => {
    const navigate = useNavigate();

    useEffect(() => {
        const params = new URLSearchParams(window.location.search);
        const accessToken = params.get('accessToken');

        if (accessToken) {
            localStorage.setItem('token', accessToken);
            onLogIn();

            setTimeout(() => {
                navigate('/');
            }, 1000);
        }
    }, [navigate]);

    return (
        <Container className="d-flex justify-content-center align-items-center" style={{ height: '100vh' }}>
            <Row>
                <Col className="text-center">
                    <Spinner
                        animation="border"
                        role="status"
                        variant="primary"
                        style={{ width: '4rem', height: '4rem' }}>
                        <span className="visually-hidden">Loading...</span>
                    </Spinner>
                    <div className="mt-3">Autorization...</div>
                </Col>
            </Row>
        </Container>
    );
};

export default AuthCallBack;
