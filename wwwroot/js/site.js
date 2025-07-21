// Enhanced JavaScript for Device Management System
// Documentation: https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification

// Global configuration
const DeviceManagement = {
    init: function() {
        this.setupFormValidation();
        this.setupLoadingStates();
        this.setupTooltips();
        this.setupAlerts();
        this.setupConfirmDialogs();
        this.setupDataTables();
    },

    // Setup form validation with visual feedback
    setupFormValidation: function() {
        $('form').on('submit', function(e) {
            const form = $(this);
            const submitBtn = form.find('button[type="submit"], input[type="submit"]');

            // Phone number regex validation
            const phoneInput = form.find('input[name="PhoneNumber"], input[asp-for="PhoneNumber"]');
            if (phoneInput.length) {
                const phoneVal = phoneInput.val();
                const phoneRegex = /^\d{3}-\d{3}-\d{4}$/;
                if (!phoneRegex.test(phoneVal)) {
                    phoneInput.addClass('is-invalid');
                    DeviceManagement.showNotification('Phone number must be in format 123-456-7890', 'error');
                    e.preventDefault();
                    return false;
                } else {
                    phoneInput.removeClass('is-invalid').addClass('is-valid');
                }
            }

            // Add loading state to submit button
            if (submitBtn.length && !submitBtn.hasClass('no-loading')) {
                const originalText = submitBtn.html();
                submitBtn.prop('disabled', true)
                         .addClass('loading-btn')
                         .html('<span class="spinner-border spinner-border-sm me-2" role="status"></span>Processing...');

                // Reset button after 5 seconds as fallback
                setTimeout(() => {
                    submitBtn.prop('disabled', false)
                             .removeClass('loading-btn')
                             .html(originalText);
                }, 5000);
            }
        });

        // Real-time validation feedback
        $('.form-control, .form-select').on('blur', function() {
            const field = $(this);
            if (field.val().trim() !== '') {
                field.removeClass('is-invalid').addClass('is-valid');
            }
        });
    },

    // Setup loading states for buttons and links
    setupLoadingStates: function() {
        $('a[href*="/Create"], a[href*="/Edit"], a[href*="/Details"]').on('click', function() {
            const link = $(this);
            if (!link.hasClass('no-loading')) {
                const originalText = link.html();
                link.html('<span class="spinner-border spinner-border-sm me-1" role="status"></span>Loading...');
                
                // Reset after navigation or timeout
                setTimeout(() => {
                    link.html(originalText);
                }, 3000);
            }
        });
    },

    // Initialize tooltips
    setupTooltips: function() {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    },

    // Auto-hide alerts with animation
    setupAlerts: function() {
        $('.alert').each(function() {
            const alert = $(this);
            
            // Auto-hide after 5 seconds
            setTimeout(() => {
                alert.fadeOut('slow', function() {
                    $(this).remove();
                });
            }, 5000);
            
            // Add dismiss animation
            alert.find('.btn-close').on('click', function() {
                alert.fadeOut('fast');
            });
        });
    },

    // Setup confirmation dialogs
    setupConfirmDialogs: function() {
        $('a[href*="/Delete"], button[data-confirm]').on('click', function(e) {
            const element = $(this);
            const message = element.data('confirm') || 'Are you sure you want to delete this item?';
            
            if (!confirm(message)) {
                e.preventDefault();
                return false;
            }
        });
    },

    // Enhanced table features
    setupDataTables: function() {
        // Add search highlighting
        $('.table').on('draw.dt search.dt', function() {
            const searchTerm = $('.dataTables_filter input').val();
            if (searchTerm) {
                $(this).find('tbody tr').each(function() {
                    const row = $(this);
                    const text = row.text();
                    if (text.toLowerCase().includes(searchTerm.toLowerCase())) {
                        row.addClass('table-warning');
                    }
                });
            }
        });

        // Row hover effects
        $('.table tbody tr').hover(
            function() {
                $(this).addClass('table-active');
            },
            function() {
                $(this).removeClass('table-active');
            }
        );
    },

    // Utility functions
    showNotification: function(message, type = 'info') {
        const alertClass = `alert-${type}`;
        const iconClass = type === 'success' ? 'bi-check-circle-fill' : 
                         type === 'error' ? 'bi-exclamation-triangle-fill' : 
                         type === 'warning' ? 'bi-exclamation-triangle-fill' : 
                         'bi-info-circle-fill';
        
        const alert = $(`
            <div class="alert ${alertClass} alert-dismissible fade show position-fixed" 
                 style="top: 20px; right: 20px; z-index: 9999; min-width: 300px;">
                <i class="${iconClass} me-2"></i>
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        `);
        
        $('body').append(alert);
        
        // Auto-remove after 4 seconds
        setTimeout(() => {
            alert.fadeOut(() => alert.remove());
        }, 4000);
    },

    // Format currency
    formatCurrency: function(amount) {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD'
        }).format(amount);
    },

    // Format date
    formatDate: function(date, format = 'dd/MM/yyyy') {
        const d = new Date(date);
        const day = String(d.getDate()).padStart(2, '0');
        const month = String(d.getMonth() + 1).padStart(2, '0');
        const year = d.getFullYear();
        
        return format.replace('dd', day).replace('MM', month).replace('yyyy', year);
    }
};

// Search functionality
const SearchManager = {
    init: function() {
        this.setupLiveSearch();
        this.setupAdvancedFilters();
    },

    setupLiveSearch: function() {
        let searchTimeout;
        $('input[name="SearchTerm"]').on('input', function() {
            clearTimeout(searchTimeout);
            const searchTerm = $(this).val();
            
            searchTimeout = setTimeout(() => {
                if (searchTerm.length >= 3 || searchTerm.length === 0) {
                    // Auto-submit form after 500ms delay
                    $(this).closest('form').submit();
                }
            }, 500);
        });
    },

    setupAdvancedFilters: function() {
        // Reset filters
        $('.btn-reset-filters').on('click', function(e) {
            e.preventDefault();
            const form = $(this).closest('form');
            form.find('input, select').val('');
            form.submit();
        });

        // Quick filter buttons
        $('.quick-filter').on('click', function(e) {
            e.preventDefault();
            const filterValue = $(this).data('filter-value');
            const filterField = $(this).data('filter-field');
            
            $(`select[name="${filterField}"]`).val(filterValue);
            $(this).closest('form').submit();
        });
    }
};

// Dashboard specific functionality
const Dashboard = {
    init: function() {
        this.setupStatCards();
        this.setupCharts();
        this.setupRefresh();
    },

    setupStatCards: function() {
        $('.dashboard-card').each(function(index) {
            $(this).css('animation-delay', `${index * 0.1}s`).addClass('fade-in-up');
        });
    },

    setupCharts: function() {
        // Placeholder for chart initialization
        console.log('Charts would be initialized here');
    },

    setupRefresh: function() {
        $('.refresh-dashboard').on('click', function() {
            location.reload();
        });
    }
};

// Initialize on document ready
$(document).ready(function() {
    DeviceManagement.init();
    SearchManager.init();
    
    // Initialize dashboard if on dashboard page
    if ($('.dashboard-card').length > 0) {
        Dashboard.init();
    }
    
    // Smooth scrolling for anchor links
    $('a[href^="#"]').on('click', function(event) {
        const target = $(this.getAttribute('href'));
        if (target.length) {
            event.preventDefault();
            $('html, body').stop().animate({
                scrollTop: target.offset().top - 80
            }, 1000, 'easeInOutExpo');
        }
    });
    
    // Back to top button
    $(window).scroll(function() {
        if ($(this).scrollTop() > 100) {
            $('#backToTop').fadeIn();
        } else {
            $('#backToTop').fadeOut();
        }
    });
    
    $('#backToTop').click(function() {
        $('html, body').animate({scrollTop: 0}, 600);
        return false;
    });
});

// Add custom animations CSS
const customStyles = `
<style>
@keyframes fadeInUp {
    from {
        opacity: 0;
        transform: translate3d(0, 30px, 0);
    }
    to {
        opacity: 1;
        transform: translate3d(0, 0, 0);
    }
}

.fade-in-up {
    animation: fadeInUp 0.6s ease-out both;
}

.loading-btn {
    pointer-events: none;
}

.pulse {
    animation: pulse 2s infinite;
}

@keyframes pulse {
    0% {
        transform: scale(1);
    }
    50% {
        transform: scale(1.05);
    }
    100% {
        transform: scale(1);
    }
}

.table-hover tbody tr:hover {
    background-color: rgba(0,123,255,.075) !important;
}

#backToTop {
    display: none;
    position: fixed;
    bottom: 20px;
    right: 20px;
    z-index: 99;
    background: #007bff;
    color: white;
    border: none;
    width: 50px;
    height: 50px;
    border-radius: 50%;
    cursor: pointer;
    transition: all 0.3s ease;
}

#backToTop:hover {
    background: #0056b3;
    transform: translateY(-2px);
}
</style>
`;

// Inject custom styles
$('head').append(customStyles);

// Add back to top button to body
$('body').append('<button id="backToTop" title="Back to Top"><i class="bi bi-arrow-up"></i></button>');
