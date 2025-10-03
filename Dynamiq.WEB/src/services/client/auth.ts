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
    const origin = window.location.origin;
    const path = window.location.pathname;
    const query = window.location.search;
    const hash = window.location.hash;

    const res = await apiRequest<LogInByGoogleResponse>(
        `auth/google/log-in?returnUrl=${origin}${path}${query}${hash}`,
        {
            method: 'GET',
        }
    );

    if (res.success && res.data) {
        window.location.href = res.data.Url;
    }

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
