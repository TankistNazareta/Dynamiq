import { ApiResult, ErrorMsgType } from '../utils/types/api';

const API_BASE = 'https://api.dynamiq-nazareta.fun';

export async function apiRequest<T>(endpoint: string, options: RequestInit = {}): Promise<ApiResult<T>> {
    const response = await fetch(`${API_BASE}${endpoint}`, {
        credentials: 'include',
        headers: {
            'Content-Type': 'application/json',
            ...(options.headers || {}),
        },
        ...options,
    });

    if (response.status === 401) {
        const refreshResponse = await fetch(`${API_BASE}/token/refresh`, {
            credentials: 'include',
        });

        if (refreshResponse.ok) return apiRequest<T>(endpoint, options);

        return {
            success: false,
            error: { StatusCode: 401, Message: 'Please restart the page (press F5)' },
        };
    }

    if (!response.ok) {
        const json = await response.json();
        return { success: false, error: json as ErrorMsgType };
    }

    return {
        success: true,
        data: (await response.json()) as T,
    };
}
