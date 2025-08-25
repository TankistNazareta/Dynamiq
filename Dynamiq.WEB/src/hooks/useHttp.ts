import { useState } from 'react';
import { apiRequest } from '../services/api';
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

            if (!res.success) throw res.error;

            return res.data;
        } catch (e: any) {
            setState('error');

            if (!navigator.onLine) {
                setState('fatal');
                navigate('/error');
                throw new Error('No internet connection');
            }

            if (e.code === 'ERR_NETWORK') {
                setState('fatal');
                navigate('/error');
                throw new Error('Server is not reachable');
            }

            throw e;
        }
    };

    const resetError = () => {
        setState('loading');
    };

    return { makeRequest, resetError, state, setState };
};

export default useHttpHook;
