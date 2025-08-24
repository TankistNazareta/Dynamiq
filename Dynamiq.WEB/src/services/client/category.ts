import { apiRequest } from '../api';

export type CategoryRes = {
    id: string;
    name: string;
    slug: string;
    subCategories: CategoryRes[];
};

export type GetCategoryWithParentByIdRes = {
    categoriesSlug: string[];
};

export const getAllCategories = async () => {
    const res = await apiRequest<CategoryRes[]>(`/category`);
    return res;
};

export const getCategoryWithParentById = async (id: string) => {
    const res = await apiRequest<GetCategoryWithParentByIdRes>(`/category/child-and-parents?id=${id}`);

    return res;
};
