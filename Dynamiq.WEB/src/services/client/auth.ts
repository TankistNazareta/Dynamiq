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

const RefreshTheAccessToken = async () => {
    const token = localStorage.getItem('token');
    if (!token) {
        return;
    }

    try {
        const res = await apiRequest<AccessTokenReturnType>('/token/refresh', { method: 'PUT' });

        if (res.success) {
            localStorage.setItem('token', res.data.accessToken);
        } else {
            throw res.error;
        }
    } catch (err) {
        console.log(err);
        await logOut();
    }
};

const logOut = async () => {
    const authResponse = await apiRequest<AccessTokenReturnType>('/auth/log-out', {
        method: 'POST',
    });

    localStorage.removeItem('token');

    return authResponse;
};

const signUp = async (email: string, password: string): Promise<ApiResult<ResponseMsg>> => {
    const authResponse = await apiRequest<ResponseMsg>('/auth/sign-up', {
        method: 'POST',
        body: JSON.stringify({ email, password }),
    });

    return authResponse;
};

export { logIn, signUp, logOut, RefreshTheAccessToken as startToRefreshAccessToken };
export type { AccessTokenReturnType };
