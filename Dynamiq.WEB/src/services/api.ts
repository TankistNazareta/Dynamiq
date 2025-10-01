import { ApiResult, ErrorMsgType } from '../utils/types/api';

const API_BASE = 'https://api.dynamiq-nazareta.fun';

export async function apiRequest<T>(endpoint: string, options: RequestInit = {}): Promise<ApiResult<T>> {
    const token = localStorage.getItem('token');

    const response = await fetch(`${API_BASE}${endpoint}`, {
        credentials: 'include',
        headers: {
            'Content-Type': 'application/json',
            ...(token ? { Authorization: `Bearer ${token}` } : {}),
            ...(options.headers || {}),
        },
        ...options,
    });

    if (!response.ok) {
        const json = await response.json();

        const error: ApiResult<T> = {
            success: false,
            error: json as ErrorMsgType,
        };

        return error;
    }

    const dataJson = (await response.json()) as T;

    const data: ApiResult<T> = {
        success: true,
        data: dataJson,
    };

    return data;
}
