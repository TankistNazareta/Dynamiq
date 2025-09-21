import { ProductFilter } from '../../services/client/product';
import SortEnum from '../enums/sortByEnum';
import { CategoryItemPorps } from '../../pages/Shop/components/CategoryItem';
import { CategoryRes } from '../../services/client/category';

const getFilterFromUrl = (searchParams: URLSearchParams, categories: CategoryItemPorps[] | CategoryRes[]) => {
    let filter: ProductFilter = {};

    const sortBy = searchParams.get('sortBy');
    const minPrice = searchParams.get('minPrice');
    const maxPrice = searchParams.get('maxPrice');
    const categoryIds = searchParams.getAll('category');
    const search = searchParams.get('search');

    if (categoryIds) {
        const foundIds = getAllIdsFromUrl(categoryIds, categories);

        if (foundIds.length) filter.categoryIds = foundIds;
    }
    if (sortBy) filter.sortBy = Number.parseInt(sortBy) as SortEnum;
    if (minPrice) filter.minPrice = Number.parseInt(minPrice);
    if (maxPrice) filter.maxPrice = Number.parseInt(maxPrice);
    if (search) filter.searchTerm = search;

    return filter;
};

const getAllIdsFromUrl = (
    parentIdsNeedToFound: string[],
    categoryItemsProp: CategoryItemPorps[] | CategoryRes[],
    foundParent: boolean = false
) => {
    let foundIds: string[] = [];

    categoryItemsProp.forEach((categoryItem) => {
        if (foundParent || parentIdsNeedToFound.includes(categoryItem.id)) {
            foundIds.push(categoryItem.id);

            if (categoryItem.subCategories.length)
                foundIds.push(...getAllIdsFromUrl(parentIdsNeedToFound, categoryItem.subCategories, true));
        } else if (categoryItem.subCategories.length)
            foundIds.push(...getAllIdsFromUrl(parentIdsNeedToFound, categoryItem.subCategories, false));
    });

    return foundIds;
};

export default getFilterFromUrl;
