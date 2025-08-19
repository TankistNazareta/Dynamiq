import { useState } from 'react';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';

const AuthPage = () => {
    const [newUser, setNewUser] = useState(false);

    return (
        <Form className="auth__window">
            <h3 className="auth_title">{newUser ? 'Sign up' : 'Log in'}</h3>
            <Form.Group className="mb-3" controlId="formBasicEmail">
                <Form.Label>Email address</Form.Label>
                <Form.Control type="email" placeholder="Enter email" />
            </Form.Group>

            <Form.Group className="mb-3" controlId="formBasicPassword">
                <Form.Label>Password</Form.Label>
                <Form.Control type="password" placeholder="Password" />
            </Form.Group>
            <div className="auth__bottom d-flex justify-content-between">
                <Button variant="primary" type="submit">
                    {newUser ? 'Sign up' : 'Log in'}
                </Button>
                <button
                    className="auth__bottom_btn"
                    onClick={(e) => {
                        e.preventDefault();
                        setNewUser(!newUser);
                    }}>
                    {newUser ? 'Already have account' : 'New user'}
                </button>
            </div>
        </Form>
    );
};

export default AuthPage;
