import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { ApiResult } from '../utils/types/api';

export type stateType = 'loading' | 'error' | 'fatal' | 'success' | 'waiting';

const useHttpHook = () => {
    const [state, setState] = useState<stateType>('waiting');
    const navigate = useNavigate();

    const makeRequest = async <T>(callMethod: () => Promise<ApiResult<T>>): Promise<T> => {
        try {
            setState('loading');

            const res = await callMethod();

            if (!res.success) {
                setState('error');
                throw res.error;
            }

            setState('success');
            return res.data;
        } catch (e: any) {
            if (!navigator.onLine) {
                setState('fatal');
                navigate('/error');
                throw new Error('No internet connection');
            }

            if (e instanceof TypeError && e.message.includes('Failed to fetch')) {
                setState('fatal');
                navigate('/error');
                throw new Error('Server is not reachable');
            }

            if (e?.status && e.status >= 500) {
                setState('fatal');
                navigate('/error');
                throw new Error(`Server error: ${e.status}`);
            }

            setState('error');
            throw e;
        }
    };

    const resetError = () => {
        setState('waiting');
    };

    return { makeRequest, resetError, state, setState };
};

export default useHttpHook;
