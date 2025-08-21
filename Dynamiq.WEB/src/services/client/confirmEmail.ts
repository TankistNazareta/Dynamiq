import { ApiResult } from '../../utils/types/api';
import { apiRequest } from '../api';

export const confirmEmail = async (token: string): Promise<ApiResult<string>> => {
    const confirmRes = await apiRequest<string>(`/email-verification/confirm?token=${token}`, {
        method: 'PUT',
    });

    return confirmRes;
};
