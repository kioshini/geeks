// axios API client
import axios from 'axios';

/**
 * API client configuration
 * Uses environment variable VITE_API_BASE_URL or defaults to localhost:5000
 */
const api = axios.create({
	baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000',
	headers: {
		'Content-Type': 'application/json',
	},
});

// Types
export type ProductDto = {
	id: number;
	name: string;
	code: string;
	description?: string;
	type: string;
	material: string;
	diameter?: number;
	length?: number;
	thickness?: number;
	unit?: string;
	price: number;
	stockQuantity: number;
	isAvailable: boolean;
	
	// Discount information
	volumeDiscountThreshold?: number;
	volumeDiscountPercent?: number;
	materialDiscountPercent?: number;
	finalPrice?: number;
	discountAmount?: number;
	totalDiscountPercent?: number;
	hasVolumeDiscount: boolean;
	hasMaterialDiscount: boolean;
};

export type ProductType = {
	id: number;
	name: string;
	description?: string;
	category?: string;
	isActive: boolean;
};

export type AddToCartDto = { productId: number; quantity: number };
export type UpdateCartItemDto = { quantity: number };
export type CartItemDto = {
	id: number;
	productId: number;
	product?: ProductDto;
	quantity: number;
	price: number;
	totalPrice: number;
};
export type CartDto = {
	userId: number;
	items: CartItemDto[];
	totalPrice: number;
	totalItems: number;
};

export type CreateOrderItemDto = { productId: number; quantity: number };
export type CreateOrderDto = {
	userId: number;
	firstName: string;
	lastName: string;
	inn?: string;
	phone: string;
	email?: string;
	items: CreateOrderItemDto[];
	notes?: string;
};
export type OrderDto = {
	id: number;
	userId: number;
	firstName: string;
	lastName: string;
	inn?: string;
	phone: string;
	email?: string;
	items: Array<{
		id: number;
		productId: number;
		product?: ProductDto;
		quantity: number;
		price: number;
		totalPrice: number;
	}>;
	totalPrice: number;
	status: string;
	notes?: string;
	createdAt: string;
	updatedAt: string;
};

/**
 * API functions for interacting with the backend
 */
export const Api = {
	/**
	 * Get all products from the catalog
	 * @returns Promise resolving to array of products
	 */
	getProducts: async () => (await api.get<ProductDto[]>('/api/products')).data,
	
	/**
	 * Get a specific product by ID
	 * @param id - Product ID
	 * @returns Promise resolving to product data
	 */
	getProduct: async (id: number) => (await api.get<ProductDto>(`/api/products/${id}`)).data,
	
	/**
	 * Search products by query string
	 * @param q - Search query
	 * @returns Promise resolving to array of matching products
	 */
	searchProducts: async (q: string) => (await api.get<ProductDto[]>(`/api/products/search`, { params: { q } })).data,
	
	/**
	 * Get all available product types/categories
	 * @returns Promise resolving to array of product types
	 */
	getProductTypes: async () => (await api.get<ProductType[]>(`/api/products/categories`)).data,

	/**
	 * Get user's shopping cart
	 * @param userId - User ID
	 * @returns Promise resolving to cart data
	 */
	getCart: async (userId: number) => (await api.get<CartDto>(`/api/cart/${userId}`)).data,
	
	/**
	 * Add item to cart
	 * @param userId - User ID
	 * @param body - Cart item data
	 * @returns Promise resolving to added cart item
	 */
	addToCart: async (userId: number, body: AddToCartDto) => (await api.post<CartItemDto>(`/api/cart/${userId}/items`, body)).data,
	
	/**
	 * Update cart item quantity
	 * @param userId - User ID
	 * @param itemId - Cart item ID
	 * @param body - Updated item data
	 * @returns Promise resolving to updated cart item
	 */
	updateCartItem: async (userId: number, itemId: number, body: UpdateCartItemDto) => (await api.put<CartItemDto>(`/api/cart/${userId}/items/${itemId}`, body)).data,
	
	/**
	 * Update cart item quantity by product ID
	 * @param userId - User ID
	 * @param productId - Product ID
	 * @param body - Updated item data
	 * @returns Promise resolving to updated cart item
	 */
	updateCartItemByProductId: async (userId: number, productId: number, body: UpdateCartItemDto) => (await api.put<CartItemDto>(`/api/cart/${userId}/items/product/${productId}`, body)).data,
	
	/**
	 * Remove item from cart
	 * @param userId - User ID
	 * @param itemId - Cart item ID
	 * @returns Promise resolving when item is removed
	 */
	removeFromCart: async (userId: number, itemId: number) => (await api.delete<void>(`/api/cart/${userId}/items/${itemId}`)).data,
	
	/**
	 * Remove item from cart by product ID
	 * @param userId - User ID
	 * @param productId - Product ID
	 * @returns Promise resolving when item is removed
	 */
	removeFromCartByProductId: async (userId: number, productId: number) => (await api.delete<void>(`/api/cart/${userId}/items/product/${productId}`)).data,
	
	/**
	 * Clear entire cart for user
	 * @param userId - User ID
	 * @returns Promise resolving when cart is cleared
	 */
	clearCart: async (userId: number) => (await api.delete<void>(`/api/cart/${userId}`)).data,

	/**
	 * Create new order
	 * @param body - Order data
	 * @param nonce - Security nonce for bot protection
	 * @returns Promise resolving to created order
	 */
	createOrder: async (body: CreateOrderDto, nonce?: string) => (await api.post<OrderDto>(`/api/orders`, body, nonce ? { headers: { 'X-Request-Nonce': nonce } } : undefined)).data,
	
	/**
	 * Get user's orders
	 * @param userId - User ID
	 * @returns Promise resolving to array of orders
	 */
	getUserOrders: async (userId: number) => (await api.get<OrderDto[]>(`/api/orders/user/${userId}`)).data,
};
