interface CartItemProps {
    imgUrl: string;
    name: string;
    quantity: number;
    priceTotal: number;
    onChangeQuantity: Function;
    onDelete: Function;
    productId: string;
}

export default CartItemProps;
