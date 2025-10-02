import './auth.scss';

import { useState } from 'react';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import { logIn, AccessTokenReturnType, signUp, logInByGoogle, LogInByGoogleResponse } from '../../services/client/auth';
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

    const onSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
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
                .catch((res) => {
                    setError(res.Message || 'Something went wrong');
                    setState('error');
                });
        }
    };

    const onLogInByGoogle = () => {
        window.location.href = 'https://api.dynamiq-nazareta.fun/auth/google/log-in';
    };

    return (
        <div className="auth__window">
            {state === 'loading' ? (
                <Loading />
            ) : (
                <>
                    <Form className="d-flex flex-column justify-content-center" onSubmit={onSubmit}>
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
                    <div className="auth__additional">
                        <button className="auth__additional_btn" onClick={() => onLogInByGoogle()}>
                            <svg
                                xmlns="http://www.w3.org/2000/svg"
                                x="0px"
                                y="0px"
                                width="25px"
                                height="25px"
                                viewBox="0 0 48 48">
                                <path
                                    fill="#fbc02d"
                                    d="M43.611,20.083H42V20H24v8h11.303c-1.649,4.657-6.08,8-11.303,8c-6.627,0-12-5.373-12-12	s5.373-12,12-12c3.059,0,5.842,1.154,7.961,3.039l5.657-5.657C34.046,6.053,29.268,4,24,4C12.955,4,4,12.955,4,24s8.955,20,20,20	s20-8.955,20-20C44,22.659,43.862,21.35,43.611,20.083z"></path>
                                <path
                                    fill="#e53935"
                                    d="M6.306,14.691l6.571,4.819C14.655,15.108,18.961,12,24,12c3.059,0,5.842,1.154,7.961,3.039	l5.657-5.657C34.046,6.053,29.268,4,24,4C16.318,4,9.656,8.337,6.306,14.691z"></path>
                                <path
                                    fill="#4caf50"
                                    d="M24,44c5.166,0,9.86-1.977,13.409-5.192l-6.19-5.238C29.211,35.091,26.715,36,24,36	c-5.202,0-9.619-3.317-11.283-7.946l-6.522,5.025C9.505,39.556,16.227,44,24,44z"></path>
                                <path
                                    fill="#1565c0"
                                    d="M43.611,20.083L43.595,20L42,20H24v8h11.303c-0.792,2.237-2.231,4.166-4.087,5.571	c0.001-0.001,0.002-0.001,0.003-0.002l6.19,5.238C36.971,39.205,44,34,44,24C44,22.659,43.862,21.35,43.611,20.083z"></path>
                            </svg>
                            Google
                        </button>
                    </div>
                </>
            )}
        </div>
    );
};

export default AuthPage;
