import './paymentStatus.scss';

import React from 'react';
import { Container, Row, Col, Button, Card } from 'react-bootstrap';
import { CheckCircle, XCircle } from 'react-bootstrap-icons';

type PaymentStatusProps = {
    status: 'success' | 'failed';
};

const PaymentStatus: React.FC<PaymentStatusProps> = ({ status }) => {
    const isSuccess = status === 'success';

    return (
        <Container className="payment-status__container">
            <Row className="justify-content-center align-items-center min-vh-100">
                <Col xs={12} md={6} lg={4}>
                    <Card className={`payment-status__card text-center p-4 ${isSuccess ? 'success' : 'failed'}`}>
                        {isSuccess ? (
                            <CheckCircle className="status-icon mb-3" />
                        ) : (
                            <XCircle className="status-icon mb-3" />
                        )}
                        <h2 className="mb-3">{isSuccess ? 'Payment Successful!' : 'Payment Failed'}</h2>
                        <p className="mb-4">
                            {isSuccess
                                ? 'Thank you for your purchase! Your payment has been processed successfully.'
                                : 'Unfortunately, your payment could not be processed. Please try again.'}
                        </p>
                        <Button variant={isSuccess ? 'success' : 'danger'} href="/">
                            Go Back Home
                        </Button>
                    </Card>
                </Col>
            </Row>
        </Container>
    );
};

export default PaymentStatus;
