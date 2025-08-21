import { ApiResult, ResponseMsg } from '../../utils/types/api';
import { apiRequest } from '../api';

type AccessTokenReturnType = {
    accessToken: string;
};

const logIn = async (email: string, password: string): Promise<ApiResult<AccessTokenReturnType>> => {
    const authResponse = await apiRequest<AccessTokenReturnType>('/auth/log-in', {
        method: 'POST',
        body: JSON.stringify({ email, password }),
    });

    return authResponse;
};

const refreshTheAccessToken = async (): Promise<ApiResult<AccessTokenReturnType>> => {
    const res = await apiRequest<AccessTokenReturnType>('/token/refresh', { method: 'PUT' });

    if (res.success) localStorage.setItem('token', await res.data.accessToken);

    return res;
};

type LogInByGoogleResponse = {
    Url: string;
};

const logInByGoogle = async () => {
    console.log('before request');

    const res = await apiRequest<LogInByGoogleResponse>('auth/google/log-in', {
        method: 'GET',
    });
    console.log('after request', res, 'res');

    if (res.success && res.data) {
        window.location.href = res.data.Url;
    }

    console.log(res, 'logInByGoogle');

    return res;
};

const logOut = async () => {
    const authResponse = await apiRequest<AccessTokenReturnType>('/auth/log-out', {
        method: 'POST',
    });

    localStorage.removeItem('token');

    console.log('logged out');

    return authResponse;
};

const signUp = async (email: string, password: string): Promise<ApiResult<ResponseMsg>> => {
    const authResponse = await apiRequest<ResponseMsg>('/auth/sign-up', {
        method: 'POST',
        body: JSON.stringify({ email, password }),
    });

    return authResponse;
};

export { logIn, signUp, logOut, refreshTheAccessToken, logInByGoogle };
export type { AccessTokenReturnType, LogInByGoogleResponse };
