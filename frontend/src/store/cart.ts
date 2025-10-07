import { create } from 'zustand';
import { Api } from '../lib/api';
import type { AddToCartDto, CartDto, UpdateCartItemDto } from '../lib/api';

export type CartState = {
	userId: number | null;
	cart: CartDto | null;
	loading: boolean;
	error?: string;

	setUserId: (id: number) => void;
	loadCart: () => Promise<void>;
	add: (dto: AddToCartDto) => Promise<void>;
	update: (itemId: number, dto: UpdateCartItemDto) => Promise<void>;
	updateByProductId: (productId: string, dto: UpdateCartItemDto) => Promise<void>;
	remove: (itemId: number) => Promise<void>;
	removeByProductId: (productId: string) => Promise<void>;
	clear: () => Promise<void>;
};

export const useCartStore = create<CartState>((set, get) => ({
	userId: null,
	cart: null,
	loading: false,
	
	setUserId: (id: number) => set({ userId: id }),

	loadCart: async () => {
		const userId = get().userId;
		if (!userId) return;
		set({ loading: true, error: undefined });
		try {
			const cart = await Api.getCart(userId);
			set({ cart });
		} catch (e: unknown) {
			const errorMessage = e instanceof Error ? e.message : 'Failed to load cart';
			set({ error: errorMessage });
		} finally {
			set({ loading: false });
		}
	},

	add: async (dto: AddToCartDto) => {
		const userId = get().userId;
		if (!userId) return;
		set({ loading: true, error: undefined });
		try {
			await Api.addToCart(userId, dto);
			await get().loadCart();
		} catch (e: unknown) {
			const errorMessage = e instanceof Error ? e.message : 'Failed to add to cart';
			set({ error: errorMessage });
		} finally {
			set({ loading: false });
		}
	},

	update: async (itemId: number, dto: UpdateCartItemDto) => {
		const userId = get().userId;
		if (!userId) return;
		set({ loading: true, error: undefined });
		try {
			await Api.updateCartItem(userId, itemId, dto);
			await get().loadCart();
		} catch (e: unknown) {
			const errorMessage = e instanceof Error ? e.message : 'Failed to update cart item';
			set({ error: errorMessage });
		} finally {
			set({ loading: false });
		}
	},

	updateByProductId: async (productId: string, dto: UpdateCartItemDto) => {
		const userId = get().userId;
		if (!userId) return;
		set({ loading: true, error: undefined });
		try {
			await Api.updateCartItemByProductId(userId, productId, dto);
			await get().loadCart();
		} catch (e: unknown) {
			const errorMessage = e instanceof Error ? e.message : 'Failed to update cart item';
			set({ error: errorMessage });
		} finally {
			set({ loading: false });
		}
	},

	remove: async (itemId: number) => {
		const userId = get().userId;
		if (!userId) return;
		set({ loading: true, error: undefined });
		try {
			await Api.removeFromCart(userId, itemId);
			await get().loadCart();
		} catch (e: unknown) {
			const errorMessage = e instanceof Error ? e.message : 'Failed to remove from cart';
			set({ error: errorMessage });
		} finally {
			set({ loading: false });
		}
	},

	removeByProductId: async (productId: string) => {
		const userId = get().userId;
		if (!userId) return;
		set({ loading: true, error: undefined });
		try {
			await Api.removeFromCartByProductId(userId, productId);
			await get().loadCart();
		} catch (e: unknown) {
			const errorMessage = e instanceof Error ? e.message : 'Failed to remove from cart';
			set({ error: errorMessage });
		} finally {
			set({ loading: false });
		}
	},

	clear: async () => {
		const userId = get().userId;
		if (!userId) return;
		set({ loading: true, error: undefined });
		try {
			await Api.clearCart(userId);
			await get().loadCart();
		} catch (e: unknown) {
			const errorMessage = e instanceof Error ? e.message : 'Failed to clear cart';
			set({ error: errorMessage });
		} finally {
			set({ loading: false });
		}
	},
}));
