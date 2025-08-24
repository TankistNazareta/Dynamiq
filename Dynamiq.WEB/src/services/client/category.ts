import { apiRequest } from '../api';

export type CategoryRes = {
    id: string;
    name: string;
    slug: string;
    subCategories: CategoryRes[];
};

export const getAllCategories = async () => {
    const res = await apiRequest<CategoryRes[]>(`/category`);
    return res;
};
