import { ReactElement, useEffect, useState } from 'react';
import SortEnum from '../../../utils/enums/sortByEnum';
import { ProductFilter } from '../../../services/client/product';
import CategoryItem, { CategoryItemPorps } from './CategoryItem';
import useHttpHook from '../../../hooks/useHttp';
import { CategoryRes, getAllCategories } from '../../../services/client/category';
import { ErrorMsgType } from '../../../utils/types/api';
import Loading from '../../../components/Loading';
import { CloseButton } from 'react-bootstrap';

interface FilterMenuProps {
    isActive: boolean;
    onFilterProp: (filter: ProductFilter) => void;
    setNeedShowFilterMenu: (needToShow: boolean) => void;
}

const FilterMenu: React.FC<FilterMenuProps> = ({ isActive, onFilterProp, setNeedShowFilterMenu }) => {
    const [error, setError] = useState('');
    const [filter, setFilter] = useState<ProductFilter>({});
    const [categoryItems, setCategoryItems] = useState<CategoryItemPorps[]>([]);

    const { state, setState, makeRequest } = useHttpHook();

    useEffect(() => {
        if (categoryItems.length) return;

        makeRequest<CategoryRes[]>(() => getAllCategories())
            .then((res: CategoryRes[]) => {
                const data = res.map((category) => createDataForChildrenCategoryFromData(category, 1));
                setCategoryItems(data);
            })
            .then(() => setState('success'))
            .catch((error: ErrorMsgType) => setError(error.Message));
    }, []);

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

            isChecked: true,
            parent: parentNumber,
            childrenCategories: [],
        };

        if (category.subCategories && category.subCategories.length !== 0) {
            const res = category.subCategories.map((subCategory) =>
                createDataForChildrenCategoryFromData(subCategory, parentNumber + 1)
            );

            categoryProp.childrenCategories = [...categoryProp.childrenCategories, ...res];
        }

        return categoryProp;
    };

    const updateCategoriesChecked = (
        categories: CategoryItemPorps[],
        id: string,
        isChecked: boolean
    ): CategoryItemPorps[] => {
        const setDescendants = (cat: CategoryItemPorps, value: boolean): CategoryItemPorps => {
            const children = (cat.childrenCategories || []).map((c) => setDescendants(c, value));
            return { ...cat, isChecked: value, childrenCategories: children };
        };

        const mapAndUpdate = (cats: CategoryItemPorps[]): CategoryItemPorps[] => {
            return cats.map((cat) => {
                if (cat.id === id) return setDescendants(cat, isChecked);
                if (cat.childrenCategories && cat.childrenCategories.length) {
                    const newChildren = mapAndUpdate(cat.childrenCategories);
                    const newIsChecked = newChildren.length ? newChildren.every((c) => c.isChecked) : cat.isChecked;
                    return { ...cat, childrenCategories: newChildren, isChecked: newIsChecked };
                }
                return cat;
            });
        };

        return mapAndUpdate(categories);
    };

    const setCheckToCategory = (id: string, isChecked: boolean) => {
        setCategoryItems((prev) => updateCategoriesChecked(prev, id, isChecked));
    };

    const onFilter = (): ProductFilter | null => {
        if (filter.minPrice !== undefined && filter.maxPrice !== undefined && filter.minPrice > filter.maxPrice) {
            setError('min price cannot be bigger than max price');
            return null;
        }

        setError('');

        const checkedCategories = GetCheckedCategories();

        const newFilter: ProductFilter = {
            ...filter,
            ...(checkedCategories.length ? { categoryIds: [...checkedCategories] } : {}),
        };

        setFilter(newFilter);

        return newFilter;
    };

    const GetCheckedCategories = (categories: CategoryItemPorps[] = categoryItems): string[] => {
        const idsCategories: string[] = [];

        for (const category of categories) {
            if (category.childrenCategories.length)
                idsCategories.push(...GetCheckedCategories(category.childrenCategories));

            if (category.isChecked) idsCategories.push(category.id);
        }

        return idsCategories;
    };

    return (
        <div className={`shop__filter ${isActive ? 'shop__filter-active' : ''}`}>
            <CloseButton onClick={() => setNeedShowFilterMenu(false)} className="shop__filter_btn-close" />;
            <h3 className="shop__filter_title">Filter</h3>
            <div className="shop__filter__section">
                <h5 className="shop__filter__section_title">Sort by:</h5>
                <div className="shop__filter__section__item d-flex align-items-center">
                    <hr className="shop__filter__section__item_hr" />
                    <p className="shop__filter__section__item_title">From lowest price</p>
                    <input
                        type="checkbox"
                        checked={filter.sortBy === SortEnum.FromLowest}
                        onChange={(e) =>
                            setFilter((prev) => ({
                                ...prev,
                                sort: e.target.checked ? SortEnum.FromLowest : SortEnum.Default,
                            }))
                        }
                        className="shop__filter__section__item_btn-checkbox"
                    />
                </div>
                <div className="shop__filter__section__item d-flex align-items-center">
                    <hr className="shop__filter__section__item_hr" />
                    <p className="shop__filter__section__item_title">From highest price</p>
                    <input
                        type="checkbox"
                        checked={filter.sortBy === SortEnum.FromHightest}
                        onChange={(e) =>
                            setFilter((prev) => ({
                                ...prev,
                                sort: e.target.checked ? SortEnum.FromHightest : SortEnum.Default,
                            }))
                        }
                        className="shop__filter__section__item_btn-checkbox"
                    />
                </div>
            </div>
            <hr className="hr-separator" />
            <div className="shop__filter__section">
                <h5 className="shop__filter__section_title">Filter:</h5>
                <div className="shop__filter__section__item d-flex align-items-center">
                    <hr className="shop__filter__section__item_hr" />
                    <p className="shop__filter__section__item_title">Highest price</p>
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
                        className="shop__filter__section__item_btn-input"
                    />
                </div>
                <div className="shop__filter__section__item d-flex align-items-center">
                    <hr className="shop__filter__section__item_hr" />
                    <p className="shop__filter__section__item_title">Lowest price</p>
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
                        className="shop__filter__section__item_btn-input"
                    />
                </div>
            </div>
            <hr className="hr-separator" />
            <div className="shop__filter__section">
                <h5 className="shop__filter__section_title">Categories:</h5>
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
            <div className="shop__filter__section-footer">
                <button
                    className="shop__filter__section-footer_btn"
                    onClick={() => {
                        const filter = onFilter();
                        if (filter !== null) onFilterProp(filter);
                    }}>
                    Search
                </button>
                <h3 className="shop__filter__section-footer_error text-danger">
                    {error ? 'error while getting categories: ' + error : ''}
                </h3>
            </div>
        </div>
    );
};

export default FilterMenu;
