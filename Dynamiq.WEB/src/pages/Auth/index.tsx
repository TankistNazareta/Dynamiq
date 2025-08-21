import { useState } from 'react';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import { logIn, AccessTokenReturnType, signUp } from '../../services/client/auth';
import { ResponseMsg } from '../../utils/types/api';
import useHttpHook from '../../hooks/useHttp';
import Loading from '../../components/Loading';

interface AuthPageProps {
    onLogIn: Function;
}

const AuthPage: React.FC<AuthPageProps> = ({ onLogIn }) => {
    const [newUser, setNewUser] = useState(false);
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [showPassword, setShowPassword] = useState(false);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');

    const { makeRequest, state, setState } = useHttpHook();

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        setError('');
        setSuccess('');

        if (!email || !password) {
            setError('Please fill in all fields');
            return;
        }

        if (!/^\S+@\S+\.\S+$/.test(email)) {
            setError('Invalid email');
            return;
        }

        if (newUser) {
            await makeRequest<ResponseMsg>(() => signUp(email, password))
                .then((res: ResponseMsg) => {
                    setSuccess('Please confirm your email and then log in');
                    setEmail('');
                    setPassword('');
                    setNewUser(false);
                })
                .then(() => setState('success'))
                .catch((res) => {
                    setError(res.Message || 'Something went wrong');
                    setState('error');
                });
        } else {
            makeRequest<AccessTokenReturnType>(() => logIn(email, password))
                .then((res: AccessTokenReturnType) => {
                    console.log(res, 'res MakeRequest AuthPage');
                    localStorage.setItem('token', res.accessToken);
                    setEmail('');
                    setPassword('');
                })
                .then(() => setState('loading'))
                .then(() => onLogIn())
                // .then(() => window.location.reload())
                .catch((res) => {
                    setError(res.Message || 'Something went wrong');
                    setState('error');
                });
        }
    };

    return (
        <div className="auth__window">
            {state === 'loading' ? (
                <Loading />
            ) : (
                <Form className="d-flex flex-column justify-content-center" onSubmit={handleSubmit}>
                    <h3 className="auth_title">{newUser ? 'Sign up' : 'Log in'}</h3>

                    <Form.Group className="mb-3" controlId="formBasicEmail">
                        <Form.Label>Email address</Form.Label>
                        <Form.Control
                            type="email"
                            placeholder="Enter email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                        />
                    </Form.Group>

                    <Form.Group className="mb-3" controlId="formBasicPassword">
                        <Form.Label>Password</Form.Label>
                        <div style={{ display: 'flex', alignItems: 'center' }}>
                            <Form.Control
                                type={showPassword ? 'text' : 'password'}
                                placeholder="Password"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                            />
                            <Button
                                variant="outline-secondary"
                                style={{ marginLeft: '8px' }}
                                onClick={() => setShowPassword((prev) => !prev)}
                                type="button">
                                {showPassword ? 'Hide' : 'Show'}
                            </Button>
                        </div>
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

                    <p className="auth_error">{error}</p>
                    <p className="auth_success">{success}</p>
                </Form>
            )}
        </div>
    );
};

export default AuthPage;
