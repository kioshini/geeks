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

// Types for new backend integration
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
	priceT: number; // Цена за тонну
	priceM: number; // Цена за метр
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

// New types for JSON data integration
export type NomenclatureEl = {
	ID: string;
	IDCat: string;
	IDType: string;
	IDTypeNew: string;
	ProductionType: string;
	IDFunctionType: string;
	Name: string;
	Gost: string;
	FormOfLength: string;
	Manufacturer: string;
	SteelGrade: string;
	Diameter: number;
	ProfileSize2: number;
	PipeWallThickness: number;
	Status: number;
	Koef: number;
};

export type PricesEl = {
	ID: string;
	IDStock: string;
	PriceT: number;
	PriceLimitT1: number;
	PriceT1: number;
	PriceLimitT2: number;
	PriceT2: number;
	PriceM: number;
	PriceLimitM1: number;
	PriceM1: number;
	PriceLimitM2: number;
	PriceM2: number;
	NDS: number;
};

export type RemnantsEl = {
	ID: string;
	IDStock: string;
	InStockT: number;
	InStockM: number;
	SoonArriveT: number;
	SoonArriveM: number;
	ReservedT: number;
	ReservedM: number;
	UnderTheOrder: boolean;
	AvgTubeLength: number;
	AvgTubeWeight: number;
};

export type StockEl = {
	IDStock: string;
	Stock: string;
	StockName: string;
	Address: string;
	Schedule: string;
	IDDivision: string;
	CashPayment: boolean;
	CardPayment: boolean;
	FIASId: string;
	OwnerInn: string;
	OwnerKpp: string;
	OwnerFullName: string;
	OwnerShortName: string;
	RailwayStation: string;
	ConsigneeCode: string;
};

export type TypeEl = {
	IDType: string;
	Type: string;
	IDParentType: string;
};

// Root types for JSON deserialization
export type NomenclatureRoot = {
	ArrayOfNomenclatureEl: NomenclatureEl[];
};

export type PricesRoot = {
	ArrayOfPricesEl: PricesEl[];
};

export type RemnantsRoot = {
	ArrayOfRemnantsEl: RemnantsEl[];
};

export type StocksRoot = {
	ArrayOfStockEl: StockEl[];
};

export type TypesRoot = {
	ArrayOfTypeEl: TypeEl[];
};

export type ProductType = {
	id: number;
	name: string;
	description?: string;
	category?: string;
	isActive: boolean;
};

export type AddToCartDto = { productId: string; quantity: number; unit?: string };
export type UpdateCartItemDto = { quantity: number; unit?: string };
export type CartItemDto = {
	id: number;
	productId: string;
	product?: ProductDto;
	quantity: number;
	unit: string;
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

export type OrderRequest = {
	firstName: string;
	lastName: string;
	INN: string;
	phone: string;
	email: string;
	comment?: string;
	OrderedItems: Array<{
		ID: string;
		Name?: string;
		quantity: number;
		unit: string;
		price: number;
	}>;
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
	updateCartItemByProductId: async (userId: number, productId: string, body: UpdateCartItemDto) => (await api.put<CartItemDto>(`/api/cart/${userId}/items/product/${productId}`, body)).data,
	
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
	removeFromCartByProductId: async (userId: number, productId: string) => (await api.delete<void>(`/api/cart/${userId}/items/product/${productId}`)).data,
	
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

	// New methods for JSON data integration
	/**
	 * Get nomenclature data from JSON
	 * @returns Promise resolving to nomenclature data
	 */
	getNomenclature: async () => {
		const response = await api.get('/api/jsondata/nomenclature');
		// API возвращает обернутые данные, нужно извлечь массив
		const data = response.data;
		if (data && data.firstItem) {
			// Если есть firstItem, значит это обернутый ответ
			// Нужно получить полные данные из JsonDataService
			const fullResponse = await api.get('/api/jsondata/nomenclature/full');
			return fullResponse.data;
		}
		return data;
	},

	/**
	 * Get prices data from JSON
	 * @returns Promise resolving to prices data
	 */
	getPrices: async () => {
		const response = await api.get('/api/jsondata/prices');
		const data = response.data;
		if (data && data.firstItem) {
			const fullResponse = await api.get('/api/jsondata/prices/full');
			return fullResponse.data;
		}
		return data;
	},

	/**
	 * Get remnants data from JSON
	 * @returns Promise resolving to remnants data
	 */
	getRemnants: async () => {
		const response = await api.get('/api/jsondata/remnants');
		const data = response.data;
		if (data && data.firstItem) {
			const fullResponse = await api.get('/api/jsondata/remnants/full');
			return fullResponse.data;
		}
		return data;
	},

	/**
	 * Get stocks data from JSON
	 * @returns Promise resolving to stocks data
	 */
	getStocks: async () => {
		const response = await api.get('/api/jsondata/stocks');
		const data = response.data;
		if (data && data.firstItem) {
			const fullResponse = await api.get('/api/jsondata/stocks/full');
			return fullResponse.data;
		}
		return data;
	},

	/**
	 * Get types data from JSON
	 * @returns Promise resolving to types data
	 */
	getTypes: async () => {
		const response = await api.get('/api/jsondata/types');
		const data = response.data;
		if (data && data.firstItem) {
			const fullResponse = await api.get('/api/jsondata/types/full');
			return fullResponse.data;
		}
		return data;
	},

	/**
	 * Validate JSON data integrity
	 * @returns Promise resolving to validation result
	 */
	validateJsonData: async () => (await api.get<{isValid: boolean, errors: string[]}>('/api/jsondata/validate')).data,

	// Delta updates methods
	/**
	 * Test price delta calculation
	 * @param currentValue - Current price value
	 * @param delta - Delta to apply
	 * @returns Promise resolving to calculation result
	 */
	testPriceDelta: async (currentValue: number, delta: number) => 
		(await api.post<{currentValue: number, delta: number, newValue: number, message: string}>('/api/deltaupdates/test-price-delta', {currentValue, delta})).data,

	/**
	 * Test stock delta calculation
	 * @param currentValue - Current stock value
	 * @param delta - Delta to apply
	 * @returns Promise resolving to calculation result
	 */
	testStockDelta: async (currentValue: number, delta: number) => 
		(await api.post<{currentValue: number, delta: number, newValue: number, message: string}>('/api/deltaupdates/test-stock-delta', {currentValue, delta})).data,

	/**
	 * Process prices delta file manually
	 * @param filePath - Path to prices delta file
	 * @returns Promise resolving to processing result
	 */
	processPricesFile: async (filePath: string) => 
		(await api.post<{message: string}>('/api/deltaupdates/process-prices-file', {filePath})).data,

	/**
	 * Process remnants delta file manually
	 * @param filePath - Path to remnants delta file
	 * @returns Promise resolving to processing result
	 */
	processRemnantsFile: async (filePath: string) => 
		(await api.post<{message: string}>('/api/deltaupdates/process-remnants-file', {filePath})).data,

	/**
	 * Start delta updates monitoring
	 * @returns Promise resolving to monitoring status
	 */
	startDeltaMonitoring: async () => (await api.post<{message: string}>('/api/deltaupdates/start-monitoring')).data,

	/**
	 * Stop delta updates monitoring
	 * @returns Promise resolving to monitoring status
	 */
	stopDeltaMonitoring: async () => (await api.post<{message: string}>('/api/deltaupdates/stop-monitoring')).data,
};
