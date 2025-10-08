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
	// Main Button functionality
	showMainButton: (text: string, onClick: () => void) => {
		try {
			WebApp.MainButton.setText(text);
			WebApp.MainButton.onClick(onClick);
			WebApp.MainButton.show();
		} catch (error) {
			console.warn('Telegram MainButton.show() failed:', error);
		}
	},
	hideMainButton: () => {
		try {
			WebApp.MainButton.hide();
		} catch (error) {
			console.warn('Telegram MainButton.hide() failed:', error);
		}
	},
	setMainButtonText: (text: string) => {
		try {
			WebApp.MainButton.setText(text);
		} catch (error) {
			console.warn('Telegram MainButton.setText() failed:', error);
		}
	},
	// Back Button functionality
	showBackButton: (onClick: () => void) => {
		try {
			WebApp.BackButton.onClick(onClick);
			WebApp.BackButton.show();
		} catch (error) {
			console.warn('Telegram BackButton.show() failed:', error);
		}
	},
	hideBackButton: () => {
		try {
			WebApp.BackButton.hide();
		} catch (error) {
			console.warn('Telegram BackButton.hide() failed:', error);
		}
	},
	// Close app
	close: () => {
		try {
			WebApp.close();
		} catch (error) {
			console.warn('Telegram WebApp.close() failed:', error);
		}
	},
	// Show alert
	showAlert: (message: string) => {
		try {
			WebApp.showAlert(message);
		} catch (error) {
			console.warn('Telegram WebApp.showAlert() failed:', error);
		}
	},
	// Show confirm
	showConfirm: (message: string, callback: (confirmed: boolean) => void) => {
		try {
			WebApp.showConfirm(message, callback);
		} catch (error) {
			console.warn('Telegram WebApp.showConfirm() failed:', error);
		}
	},
	// Show popup
	showPopup: (params: { title?: string; message: string; buttons?: Array<{ id?: string; type: 'default' | 'destructive'; text: string }> }, callback?: (id?: string) => unknown) => {
		try {
			WebApp.showPopup(params, callback);
		} catch (error) {
			console.warn('Telegram WebApp.showPopup() failed:', error);
		}
	},
};
