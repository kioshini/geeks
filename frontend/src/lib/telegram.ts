import WebApp from '@twa-dev/sdk';

export type TgUser = {
	id: number;
	first_name?: string;
	last_name?: string;
	username?: string;
	language_code?: string;
	is_bot?: boolean;
};

export const Telegram = {
	ready: () => {
		try { 
			WebApp.ready(); 
		} catch (error) {
			console.warn('Telegram WebApp.ready() failed:', error);
		}
	},
	expand: () => {
		try { 
			WebApp.expand(); 
		} catch (error) {
			console.warn('Telegram WebApp.expand() failed:', error);
		}
	},
	theme: () => WebApp.colorScheme,
	user: (): TgUser | undefined => {
		// Preferred source: initDataUnsafe.user
		const u = WebApp.initDataUnsafe?.user as unknown;
		if (u && typeof u === 'object' && u !== null && 'id' in u && typeof (u as { id: unknown }).id === 'number') {
			return u as TgUser;
		}
		return undefined;
	},
	haptic: (type: 'impact' | 'notification' | 'selection' = 'selection') => {
		try {
			if (type === 'impact') WebApp.HapticFeedback.impactOccurred('light');
			else if (type === 'notification') WebApp.HapticFeedback.notificationOccurred('success');
			else WebApp.HapticFeedback.selectionChanged();
		} catch (error) {
			console.warn('Telegram haptic feedback failed:', error);
		}
	},
};
