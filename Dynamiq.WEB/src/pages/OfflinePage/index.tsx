import React, { useEffect, useState } from 'react';
import { Button } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';

const OfflinePage = () => {
    const navigate = useNavigate();
    const [needToGoBack, setNeedToGoBack] = useState(false);

    const reloadPage = () => {
        navigate(-1);
    };

    const goHome = () => {
        navigate('/');
    };

    return (
        <div
            style={{
                display: 'flex',
                flexDirection: 'column',
                justifyContent: 'center',
                alignItems: 'center',
                height: '100vh',
                textAlign: 'center',
                backgroundColor: '#f8f9fa',
                padding: '20px',
            }}>
            <h1 style={{ fontSize: '48px', marginBottom: '20px', color: '#dc3545' }}>😕 Oops!</h1>
            <h2 style={{ marginBottom: '15px' }}>Something went wrong</h2>
            <p style={{ maxWidth: '400px', marginBottom: '30px', color: '#6c757d' }}>
                It looks like your internet connection is down or our server is currently unavailable. Please check your
                connection or try again later.
            </p>
            <div style={{ display: 'flex', gap: '10px' }}>
                <Button variant="primary" onClick={reloadPage}>
                    Reload
                </Button>
                <Button variant="secondary" onClick={goHome}>
                    Go Home
                </Button>
            </div>
        </div>
    );
};

export default OfflinePage;
