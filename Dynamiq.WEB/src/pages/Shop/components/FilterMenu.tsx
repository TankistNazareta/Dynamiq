import { useEffect, useState } from 'react';
import SortEnum from '../../../utils/enums/sortByEnum';
import { ProductFilter } from '../../../services/client/product';
import CategoryItem, { CategoryItemPorps } from './CategoryItem';
import useHttpHook from '../../../hooks/useHttp';
import { CategoryRes, getAllCategories } from '../../../services/client/category';
import { ErrorMsgType } from '../../../utils/types/api';
import Loading from '../../../components/Loading';
import { CloseButton } from 'react-bootstrap';
import { useLocation, useSearchParams } from 'react-router-dom';
import getFilterFromUrl from '../../../utils/services/getFilterFromUrl';

interface FilterMenuProps {
    isActive: boolean;
    onFilterProp: (filter: ProductFilter) => void;
    setNeedShowFilterMenu: (needToShow: boolean) => void;
    onClickFilter: () => void;
}

const FilterMenu: React.FC<FilterMenuProps> = ({ isActive, onFilterProp, setNeedShowFilterMenu, onClickFilter }) => {
    const [error, setError] = useState('');
    const [filter, setFilter] = useState<ProductFilter>({});
    const [categoryItems, setCategoryItems] = useState<CategoryItemPorps[]>([]);
    const [categoriesIsLoading, setCategoriesIsLoading] = useState(true);

    const { state, setState, makeRequest } = useHttpHook();

    const [searchParams, setSearchParams] = useSearchParams();
    const location = useLocation();

    useEffect(() => {
        if (categoriesIsLoading) return;

        updateFilterFromUrl();
    }, [searchParams]);

    useEffect(() => {
        if (!categoryItems.length) {
            makeRequest<CategoryRes[]>(() => getAllCategories())
                .then((res: CategoryRes[]) => {
                    const data = res.map((category) => createDataForChildrenCategoryFromData(category, 1));
                    setCategoriesIsLoading(false);
                    setCategoryItems(data);
                })
                .then(() => setState('success'))
                .catch((error: ErrorMsgType) => setError(error.Message));
        }
    }, []);

    const updateFilterFromUrl = () => {
        var newFilter = getFilterFromUrl(searchParams, categoryItems);

        if (newFilter.categoryIds) newFilter.categoryIds.forEach((id) => setCheckToCategory(id, true));

        onFilterProp(newFilter);
    };

    useEffect(() => {
        if (categoriesIsLoading) return;

        setFilter(getFilterFromUrl(searchParams, categoryItems));

        updateFilterFromUrl();
    }, [categoryItems]);

    const createDataForChildrenCategoryFromData = (category: CategoryRes, parentNumber: number): CategoryItemPorps => {
        const categoryProp: CategoryItemPorps = {
            id: category.id,
            name: category.name,
            onRemove: (id: string) => {
                setCheckToCategory(id, false);
            },

            onSelect: (id: string) => {
                setCheckToCategory(id, true);
            },

            isChecked: false,
            parent: parentNumber,
            subCategories: [],
        };

        if (category.subCategories && category.subCategories.length !== 0) {
            const res = category.subCategories.map((subCategory) =>
                createDataForChildrenCategoryFromData(subCategory, parentNumber + 1)
            );

            categoryProp.subCategories = [...categoryProp.subCategories, ...res];
        }

        return categoryProp;
    };

    const updateCategoriesChecked = (
        categories: CategoryItemPorps[],
        id: string,
        isChecked: boolean
    ): CategoryItemPorps[] => {
        const setDescendants = (cat: CategoryItemPorps, value: boolean): CategoryItemPorps => {
            const children = (cat.subCategories || []).map((c) => setDescendants(c, value));
            return { ...cat, isChecked: value, subCategories: children };
        };

        const mapAndUpdate = (cats: CategoryItemPorps[]): CategoryItemPorps[] => {
            return cats.map((cat) => {
                if (cat.id === id) return setDescendants(cat, isChecked);
                if (cat.subCategories && cat.subCategories.length) {
                    const newChildren = mapAndUpdate(cat.subCategories);
                    const newIsChecked = newChildren.length ? newChildren.every((c) => c.isChecked) : cat.isChecked;
                    return { ...cat, subCategories: newChildren, isChecked: newIsChecked };
                }
                return cat;
            });
        };

        return mapAndUpdate(categories);
    };

    const setCheckToCategory = (id: string, isChecked: boolean) => {
        setCategoryItems((prev) => updateCategoriesChecked(prev, id, isChecked));
    };

    const GetCheckedCategoriesForUrl = (categories: CategoryItemPorps[] = categoryItems) => {
        const idsCategories: string[] = [];

        for (const category of categories) {
            if (category.isChecked) idsCategories.push(category.id);
            else if (category.subCategories.length)
                idsCategories.push(...GetCheckedCategoriesForUrl(category.subCategories));
        }

        return idsCategories;
    };

    const onFilter = () => {
        if (filter.minPrice !== undefined && filter.maxPrice !== undefined && filter.minPrice > filter.maxPrice) {
            setError('min price cannot be bigger than max price');
            return null;
        }

        setError('');

        // url
        const urlSearchParams = new URLSearchParams(searchParams);

        if (filter.sortBy !== undefined && filter.sortBy !== SortEnum.Default)
            urlSearchParams.set('sortBy', filter.sortBy.toString());
        else urlSearchParams.delete('sortBy');

        if (filter.maxPrice) urlSearchParams.set('maxPrice', filter.maxPrice.toString());
        else urlSearchParams.delete('maxPrice');

        if (filter.minPrice) urlSearchParams.set('minPrice', filter.minPrice.toString());
        else urlSearchParams.delete('minPrice');

        const checkedCategoriesForUrl = GetCheckedCategoriesForUrl();

        urlSearchParams.delete('category');

        if (checkedCategoriesForUrl.length) {
            checkedCategoriesForUrl.forEach((c) => urlSearchParams.append('category', c));
        }

        setSearchParams(urlSearchParams);
    };

    const GetCheckedCategories = (categories: CategoryItemPorps[] = categoryItems): string[] => {
        const idsCategories: string[] = [];

        for (const category of categories) {
            if (category.subCategories.length) idsCategories.push(...GetCheckedCategories(category.subCategories));

            if (category.isChecked) idsCategories.push(category.id);
        }

        return idsCategories;
    };

    return (
        <div className={`filter ${isActive ? 'filter--active' : ''}`}>
            <CloseButton onClick={() => setNeedShowFilterMenu(false)} className="filter_btn-close" />
            <h3 className="filter__title">Filter</h3>
            <div className="filter__section">
                <h5 className="filter__section-title">Sort by:</h5>
                <div className="filter__section-item d-flex align-items-center">
                    <hr className="filter__section-item__hr" />
                    <p className="filter__section-item__title">From lowest price</p>
                    <input
                        type="checkbox"
                        checked={filter.sortBy === SortEnum.FromLowest}
                        onChange={(e) =>
                            setFilter((prev) => ({
                                ...prev,
                                sortBy: e.target.checked ? SortEnum.FromLowest : SortEnum.Default,
                            }))
                        }
                        className="filter__section-item__btn-checkbox"
                    />
                </div>
                <div className="filter__section-item d-flex align-items-center">
                    <hr className="filter__section-item__hr" />
                    <p className="filter__section-item__title">From highest price</p>
                    <input
                        type="checkbox"
                        checked={filter.sortBy === SortEnum.FromHightest}
                        onChange={(e) =>
                            setFilter((prev) => ({
                                ...prev,
                                sortBy: e.target.checked ? SortEnum.FromHightest : SortEnum.Default,
                            }))
                        }
                        className="filter__section-item__btn-checkbox"
                    />
                </div>
            </div>
            <hr className="hr-separator" />
            <div className="filter__section">
                <h5 className="filter__section__title">Filter:</h5>
                <div className="filter__section-item d-flex align-items-center">
                    <hr className="filter__section-item__hr" />
                    <p className="filter__section-item__title">Highest price</p>
                    <input
                        type="text"
                        value={filter.maxPrice ?? 0}
                        onChange={(e) => {
                            const onlyNums = e.target.value.replace(/\D/g, '');
                            setFilter((prev) => ({
                                ...prev,
                                maxPrice: e.target.value ? Number.parseInt(onlyNums) : undefined,
                            }));
                        }}
                        className="filter__section-item__btn-input"
                    />
                </div>
                <div className="filter__section-item d-flex align-items-center">
                    <hr className="filter__section-item__hr" />
                    <p className="filter__section-item__title">Lowest price</p>
                    <input
                        type="text"
                        value={filter.minPrice ?? 0}
                        onChange={(e) => {
                            const onlyNums = e.target.value.replace(/\D/g, '');
                            setFilter((prev) => ({
                                ...prev,
                                minPrice: e.target.value ? Number.parseInt(onlyNums) : undefined,
                            }));
                        }}
                        className="filter__section-item__btn-input"
                    />
                </div>
            </div>
            <hr className="hr-separator" />
            <div className="filter__section">
                <h5 className="filter__section-title">Categories:</h5>
                {state == 'loading' ? (
                    <Loading />
                ) : state == 'success' ? (
                    <>
                        {categoryItems?.map((item) => (
                            <CategoryItem key={item.id} {...item} />
                        ))}
                    </>
                ) : (
                    ''
                )}
            </div>
            <div className="filter__footer">
                <button
                    className="filter__footer-btn"
                    onClick={() => {
                        onFilter();
                        onClickFilter();
                    }}>
                    Search
                </button>
                <h3 className="filter__footer-error text-danger">
                    {error ? 'error while getting categories: ' + error : ''}
                </h3>
            </div>
        </div>
    );
};

export default FilterMenu;
