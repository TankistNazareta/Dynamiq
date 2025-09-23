import { ReactElement, useEffect, useState } from 'react';
import useHttpHook from '../../hooks/useHttp';
import Loading from '../Loading';
import {
    getFilteredProducts,
    getRangeProduct,
    ProductFilter,
    ProductRes,
    ProductResBody,
} from '../../services/client/product';
import Card from '.';

interface CardListProps {
    className?: string;
    limit: number;
    offset: number;
    productFilter?: ProductFilter;
    setTotalCount?: (count: number) => void;
}

const CardList: React.FC<CardListProps> = ({ className, limit, offset, productFilter, setTotalCount }) => {
    const [error, setError] = useState('');
    const [products, setProducts] = useState<ProductResBody[]>();

    const { state, setState, makeRequest } = useHttpHook();

    useEffect(() => {
        const requestMethod =
            productFilter != null
                ? () => getFilteredProducts(productFilter, limit, offset)
                : () => getRangeProduct(limit, offset);

        console.log(productFilter);

        makeRequest<ProductRes>(requestMethod)
            .then((res) => {
                console.log('CardList made request (looking for perfomance)');
                if (setTotalCount !== undefined) setTotalCount(res.totalCount);
                setProducts(res.products);
            })
            .then(() => setState('success'))
            .catch((err: any) => {
                console.error(err);
                setError(err?.message || 'Unknown error');
            });
    }, [offset, limit, productFilter]);

    return (
        <div className={`d-flex flex-wrap justify-content-between gx-4 gy-4 ${className}`}>
            {state === 'success' ? (
                products?.map((prod) => (
                    <Card
                        key={prod.id}
                        additionalClasses="main__card"
                        name={prod.name}
                        price={prod.price}
                        descr={prod.cardDescription}
                        imgUrl={prod.imgUrls[0].imgUrl}
                        id={prod.id}
                    />
                ))
            ) : state === 'error' ? (
                <h3 className="text-danger">Error while getting data: {error}</h3>
            ) : (
                <Loading />
            )}
        </div>
    );
};

export default CardList;
