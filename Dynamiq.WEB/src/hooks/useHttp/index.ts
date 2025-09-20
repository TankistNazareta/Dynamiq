import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { ApiResult, ErrorMsgType } from '../../utils/types/api';
import { useInfoMsg } from '../../components/InfoMsg/InfoMsgContext';

export type stateType = 'loading' | 'error' | 'fatal' | 'success' | 'waiting';

const useHttpHook = () => {
    const [state, setState] = useState<stateType>('waiting');
    const navigate = useNavigate();
    const { addItem } = useInfoMsg();

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
            const setFatalError = (msg: string) => {
                setState('fatal');
                navigate('/error');
                addItem({ type: 'error', msg });
                throw new Error(msg);
            };

            if (!navigator.onLine) setFatalError('No internet connection');

            if (e instanceof TypeError && e.message.includes('Failed to fetch'))
                setFatalError('Server is not reachable');

            if (e?.status && e.status >= 500) setFatalError(`Server error: ${e.status}`);

            if (e.errors) {
                const firstField = Object.keys(e.errors)[0];
                const firstError = e.errors[firstField][0];

                const error: ErrorMsgType = {
                    StatusCode: e.status,
                    Message: firstError,
                };

                e = error;
            }

            const error = e as ErrorMsgType;
            if (error?.StatusCode && error.StatusCode !== 404) addItem({ type: 'error', msg: error.Message });

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
